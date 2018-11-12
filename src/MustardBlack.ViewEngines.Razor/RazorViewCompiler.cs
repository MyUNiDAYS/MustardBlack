using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Hosting.Internal;
using MustardBlack.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.CodeAnalysis.Text;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MustardBlack.ViewEngines.Razor
{
	public class RazorViewCompiler
	{
		readonly IFileSystem fileSystem;
		readonly IRazorConfiguration razorConfiguration;

		readonly string[] defaultAssemblies =
		{
			GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite).Assembly)
			//,GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly)
		};

		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		readonly RazorTemplateEngine razorTemplateEngine;
		readonly RazorSourceDocument[] defaultImports;
		List<MetadataReference> referenceAssemblies;
		readonly CSharpCompiler csharpCompiler;

		public RazorViewCompiler(IFileSystem fileSystem, IRazorConfiguration razorConfiguration)
		{
			this.fileSystem = fileSystem;
			this.razorConfiguration = razorConfiguration;
			this.razorTemplateEngine = BuildRazorTemplateEngine(RazorProjectFileSystem.Create(this.fileSystem.GetFullPath("~/")));
			var defaultTagHelpers = razorConfiguration.GetDefaultTagHelpers();
			var defaultDirectivesProjectItem = new DefaultDirectivesProjectItem(this.razorConfiguration.GetDefaultNamespaces(), defaultTagHelpers);
			this.defaultImports = new[] {RazorSourceDocument.ReadFrom(defaultDirectivesProjectItem)};
			this.GetReferenceAssemblies();
			
			this.csharpCompiler = new CSharpCompiler(new HostingEnvironment(), this.referenceAssemblies);

		}

		void GetReferenceAssemblies()
		{
			var assemblies = new List<string>();
			var appAssembly = this.razorConfiguration.GetApplicationAssembly();
			assemblies.AddRange(this.defaultAssemblies);
			assemblies.Add(GetAssemblyPath(appAssembly)); // current app
			assemblies.AddRange(appAssembly.GetReferencedAssemblies().Select(GetAssemblyPath)); // assemblies referenced by current app
			assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(GetAssemblyPath)); // loaded assemblies (superset of above line?)

			this.referenceAssemblies = assemblies
				.Distinct(p => Path.GetFileName(p).ToLowerInvariant())
				.Select(a => MetadataReference.CreateFromFile(a) as MetadataReference)
				.ToList();
		}

		static RazorTemplateEngine BuildRazorTemplateEngine(RazorProjectFileSystem fileSystem)
		{
			var razorConfiguration = Microsoft.AspNetCore.Razor.Language.RazorConfiguration.Default;
			var razorProjectEngine = RazorProjectEngine.Create(razorConfiguration, fileSystem, builder =>
			{
				FunctionsDirective.Register(builder);
				InheritsDirective.Register(builder);
				SectionDirective.Register(builder);

				var metadataReferences = AppDomain.CurrentDomain.GetAssemblies()
					.Where(a => !a.IsDynamic)
					.Select(a => MetadataReference.CreateFromFile(a.Location))
					.ToArray();

				builder.Features.Add(new DefaultMetadataReferenceFeature {References = metadataReferences});
				builder.Features.Add(new CompilationTagHelperFeature());
				builder.Features.Add(new DefaultTagHelperDescriptorProvider());
				//builder.Features.Add(new ViewComponentTagHelperDescriptorProvider());
				builder.Features.Add(new DocumentClassifierPass());
				//builder.Features.Add(new ViewComponentTagHelperPass());
			});

			var templateEngine = new RazorTemplateEngine(razorProjectEngine.Engine, fileSystem);
			return templateEngine;
		}

		static string GetAssemblyPath(Assembly assembly)
		{
			return new Uri(assembly.EscapedCodeBase).LocalPath;
		}

		static string GetAssemblyPath(AssemblyName assembly)
		{
			return GetAssemblyPath(Assembly.Load(assembly));
		}


		public Type CompileFile(RazorViewCompilationData view)
		{
			return this.CompileCSharp(view, view.ViewContents);
		}
		
		Type CompileCSharp(RazorViewCompilationData view, string source)
		{
			//
			//
			//			var compilerParameters = new CompilerParameters(this.referenceAssemblies.ToArray());
			//			compilerParameters.IncludeDebugInformation = true;
			//			compilerParameters.TempFiles.KeepFiles = false;

			var sourceDocument = RazorSourceDocument.Create(view.ViewContents, view.FilePath, Encoding.UTF8);
			var codeDocument = RazorCodeDocument.Create(sourceDocument, this.defaultImports);

			codeDocument.Items.Add("ViewCompilationData", view);
			var razorCSharpDocument = this.razorTemplateEngine.GenerateCode(codeDocument);

			log.Debug("Compiling {viewPath} from {source} as {generatedCode}", view.FilePath, source, razorCSharpDocument.GeneratedCode);

			var compiledAssembly = CompileAndEmit(codeDocument, razorCSharpDocument.GeneratedCode);
//
//			var compilationResults = this.codeProvider.CompileAssemblyFromSource(compilerParameters, razorCSharpDocument.GeneratedCode);
//			if (compilationResults.Errors.HasErrors)
//			{
//				var errors = compilationResults.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).Select(error => $"[{error.ErrorNumber}] Line: {error.Line} Column: {error.Column} - {error.ErrorText}").Aggregate((s1, s2) => s1 + "\n" + s2);
//				throw new ViewRenderException("Failed to compile view `" + view.FilePath + "`: " + errors, source, razorCSharpDocument.GeneratedCode);
//			}

			var type = compiledAssembly.GetType(view.Namespace + "." + view.ClassName);
			if (type == null)
				throw new ViewRenderException($"Could not find type `{view.Namespace + "." + view.ClassName}` in assembly `{compiledAssembly.FullName}`");

			return type;
		}


		Assembly CompileAndEmit(RazorCodeDocument codeDocument, string generatedCode)
		{
			//_logger.GeneratedCodeToAssemblyCompilationStart(codeDocument.Source.FilePath);

			//var startTimestamp = _logger.IsEnabled(LogLevel.Debug) ? Stopwatch.GetTimestamp() : 0;

			var assemblyName = Path.GetRandomFileName();
			var compilation = CreateCompilation(generatedCode, assemblyName);

			var emitOptions = this.csharpCompiler.EmitOptions;
			var emitPdbFile = this.csharpCompiler.EmitPdb && emitOptions.DebugInformationFormat != DebugInformationFormat.Embedded;

			using (var assemblyStream = new MemoryStream())
			using (var pdbStream = emitPdbFile ? new MemoryStream() : null)
			{
				var result = compilation.Emit(
					assemblyStream,
					pdbStream,
					options: emitOptions);

				if (!result.Success)
				{
					throw CompilationFailedExceptionFactory.Create(codeDocument, generatedCode, assemblyName, result.Diagnostics);
				}

				assemblyStream.Seek(0, SeekOrigin.Begin);
				pdbStream?.Seek(0, SeekOrigin.Begin);

				var assembly = Assembly.Load(assemblyStream.ToArray(), pdbStream?.ToArray());
				//_logger.GeneratedCodeToAssemblyCompilationEnd(codeDocument.Source.FilePath, startTimestamp);

				return assembly;
			}
		}

		CSharpCompilation CreateCompilation(string compilationContent, string assemblyName)
		{
			var sourceText = SourceText.From(compilationContent, Encoding.UTF8);
			var syntaxTree = this.csharpCompiler.CreateSyntaxTree(sourceText).WithFilePath(assemblyName);
			var compilation = this.csharpCompiler.CreateCompilation(assemblyName).AddSyntaxTrees(syntaxTree);
			compilation = ExpressionRewriter.Rewrite(compilation);

			var compilationContext = new RoslynCompilationContext(compilation);
			//_compilationCallback(compilationContext);
			compilation = compilationContext.Compilation;
			return compilation;
		}

		public void CompileAndMergeFiles(IEnumerable<RazorViewCompilationData> viewCompilationDetails, string outputAssemblyName)
		{
//			var outputAssemblyPath = Path.Combine(this.razorConfiguration.OutPath, outputAssemblyName + ".Views.Razor.dll");
//
//			var razorCSharpDocuments = viewCompilationDetails
//				.Select(this.GenerateCSharp)
//				.Select(d => d.GeneratedCode)
//				.ToArray();
//
//			var compilerParameters = new CompilerParameters(this.referenceAssemblies.ToArray());
//			compilerParameters.IncludeDebugInformation = true;
//			compilerParameters.TempFiles.KeepFiles = false;
//			compilerParameters.OutputAssembly = outputAssemblyPath;
//
//			var compilationResults = this.codeProvider.CompileAssemblyFromSource(compilerParameters, razorCSharpDocuments);
//			if (compilationResults.Errors.HasErrors)
//			{
//				var errors = compilationResults.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).Select(error => $"[{error.ErrorNumber}] File: {error.FileName} Line: {error.Line} Column: {error.Column} - {error.ErrorText}").Aggregate((s1, s2) => s1 + "\n" + s2);
//				throw new ViewRenderException("Failed to compile view: " + errors);
//			}
		}

		/// <summary>
		/// Returns the full paths of all the component files associated with the given view path, with the given extensions
		/// </summary>
		/// <param name="viewPath"></param>
		/// <param name="ext"></param>
		/// <returns></returns>
		public IReadOnlyList<string> GetViewComponentPaths(string viewPath, string ext)
		{
			var folder = Path.GetDirectoryName(viewPath);
			var viewName = Path.GetFileNameWithoutExtension(viewPath).ToLowerInvariant();

			var files = this.fileSystem.GetFiles(folder);
			return files.Where(f =>
			{
				var file = Path.GetFileName(f).ToLowerInvariant();
				if (file == viewName + ext)
					return true;

				if (file.StartsWith(viewName + ".") && file.EndsWith(ext))
					return true;

				return false;
			}).ToArray();
		}

		public static string GetSafeClassName(string input)
		{
			var safe = input.Replace('.', '_').Replace('-', '_');
			if (safe[0] >= '0' && safe[0] <= '9')
				safe = '_' + safe;

			return safe;
		}
	}
}