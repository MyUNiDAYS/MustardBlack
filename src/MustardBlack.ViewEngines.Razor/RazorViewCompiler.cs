using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MustardBlack.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Razor;
using Serilog;

namespace MustardBlack.ViewEngines.Razor
{
	public class RazorViewCompiler
	{
		readonly IFileSystem fileSystem;
	    readonly IRazorConfiguration razorConfiguration;

	    readonly string[] defaultAssemblies =
		{
			GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite).Assembly),
			GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly),
			//GetAssemblyPath(typeof(IHtmlString).Assembly)
		};

		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		readonly RazorTemplateEngine razorTemplateEngine;
		readonly RazorSourceDocument[] defaultImports;
		List<MetadataReference> referenceAssemblies;

		public RazorViewCompiler(IFileSystem fileSystem, IRazorConfiguration razorConfiguration)
		{
			this.fileSystem = fileSystem;
		    this.razorConfiguration = razorConfiguration;
			this.razorTemplateEngine = BuildRazorTemplateEngine(RazorProjectFileSystem.Create(this.fileSystem.GetFullPath("~/")));
			var defaultTagHelpers = razorConfiguration.GetDefaultTagHelpers();
			var defaultDirectivesProjectItem = new DefaultDirectivesProjectItem(this.razorConfiguration.GetDefaultNamespaces(), defaultTagHelpers);
			this.defaultImports = new[] { RazorSourceDocument.ReadFrom(defaultDirectivesProjectItem) };
			this.GetReferenceAssemblies();
		}

		void GetReferenceAssemblies()
		{
			var assemblies = new List<string>();
			var appAssembly = GetApplicationAssembly();
			assemblies.AddRange(this.defaultAssemblies);
			assemblies.Add(GetAssemblyPath(appAssembly)); // current app
			assemblies.AddRange(appAssembly.GetReferencedAssemblies().Select(GetAssemblyPath)); // assemblies referenced by current app
			assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(GetAssemblyPath)); // loaded assemblies (superset of above line?)
			
			this.referenceAssemblies = assemblies
				.Distinct(p => Path.GetFileName(p).ToLowerInvariant())
				.Select(p => MetadataReference.CreateFromFile(p) as MetadataReference)
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

				builder.Features.Add(new DefaultMetadataReferenceFeature { References = metadataReferences });
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

		static Assembly GetApplicationAssembly()
		{
			// Try the EntryAssembly, this doesn't work for ASP.NET apps
			var ass = Assembly.GetEntryAssembly();

			// Look for web application assembly
//			var ctx = HttpContext.Current;
//			if (ctx != null)
//				ass = GetWebApplicationAssembly(ctx);

			// Fallback to executing assembly
			return ass ?? (Assembly.GetExecutingAssembly());
		}
		
//		static Assembly GetWebApplicationAssembly(HttpContext context)
//		{
//			object app = context.ApplicationInstance;
//			if (app == null) return null;
//
//			var type = app.GetType();
//			// TODO: suspect "ASP" is no longer real/correct
//			while (type != null && type != typeof(object) && type.Namespace == "ASP")
//				type = type.BaseType;
//
//			return type.Assembly;
//		}

		public Type CompileFile(RazorViewCompilationData view)
		{
			var razorCSharpDocument = this.GenerateCSharp(view);
			return this.CompileCSharp(view, view.ViewContents, razorCSharpDocument);
		}

		RazorCSharpDocument GenerateCSharp(RazorViewCompilationData view)
		{
			var sourceDocument = RazorSourceDocument.Create(view.ViewContents, view.FilePath, Encoding.UTF8);
			var codeDocument = RazorCodeDocument.Create(sourceDocument, this.defaultImports);

			codeDocument.Items.Add("ViewCompilationData", view);
			var razorCSharpDocument = this.razorTemplateEngine.GenerateCode(codeDocument);
			return razorCSharpDocument;
		}

		Type CompileCSharp(RazorViewCompilationData view, string source, RazorCSharpDocument razorCSharpDocument)
		{
			var cSharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			var compilation = CSharpCompilation.Create("assembly",
				new[] { CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode, path: view.FilePath, encoding: Encoding.UTF8) },
				this.referenceAssemblies,
				cSharpCompilationOptions);

			log.Debug("Compiling {viewPath} - 1 of 3 log entries - Broken into entries 3 due to Seq raw payload limits. If you don't see log parts 2 and/or 3. They're too big, soz", view.FilePath);
			log.Debug("Compiling {viewPath} - 2 of 3 log entries - Source {source}", view.FilePath, source);
			log.Debug("Compiling {viewPath} - 3 of 3 log entries - GeneratedCode {generatedCode}", view.FilePath, razorCSharpDocument.GeneratedCode);

			using (var assemblyStream = new MemoryStream())
			using (var symbolStream = new MemoryStream())
			{
				var result = compilation.Emit(assemblyStream, symbolStream);

				var errors = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
				if (errors.Any())
				{
					var message = errors.Select(e =>
						{
							var fileLinePositionSpan = e.Location.GetMappedLineSpan();
							return $"[{e.Id}] File: {fileLinePositionSpan.Path}, Line: {fileLinePositionSpan.StartLinePosition.Line}, Character: {fileLinePositionSpan.StartLinePosition.Character}: `{e.GetMessage()}`";
						}).Aggregate((s1, s2) => s1 + "\n" + s2);

					throw new ViewRenderException("Failed to compile view `" + view.FilePath + "`: " + message, source, razorCSharpDocument.GeneratedCode);
				}

				var assembly = Assembly.Load(assemblyStream.ToArray(), symbolStream.ToArray());

				var type = assembly.GetType(view.Namespace + "." + view.ClassName);
				if (type == null)
					throw new ViewRenderException($"Could not find type `{view.Namespace + "." + view.ClassName}` in assembly `{assembly.FullName}`");

				return type;
			}
		}

		public void CompileAndMergeFiles(IEnumerable<RazorViewCompilationData> viewCompilationDetails, string outputAssemblyName)
		{
			var moduleName = outputAssemblyName + ".Views.Razor";
			var outputAssemblyPath = Path.Combine(this.razorConfiguration.OutPath, moduleName + ".dll");

			var syntaxTrees = viewCompilationDetails.Select(d =>
			{
				var razorCSharpDocument = this.GenerateCSharp(d);
				return CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode, path: d.FilePath, encoding: Encoding.UTF8);
			}).ToArray();
			
			var cSharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			var compilation = CSharpCompilation.Create(moduleName, syntaxTrees, this.referenceAssemblies, cSharpCompilationOptions);
			
			using (var assemblyStream = new MemoryStream())
			{
				var result = compilation.Emit(assemblyStream);

				var errors = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
				if (errors.Any())
				{
					var message = errors.Select(e =>
					{
						var fileLinePositionSpan = e.Location.GetMappedLineSpan();
						return $"[{e.Id}] File: {fileLinePositionSpan.Path}, Line: {fileLinePositionSpan.StartLinePosition.Line}, Character: {fileLinePositionSpan.StartLinePosition.Character}: `{e.GetMessage()}`";
					}).Aggregate((s1, s2) => s1 + "\n" + s2);

					throw new ViewRenderException("Failed to compile views: " + message);
				}

				this.fileSystem.Write(assemblyStream, outputAssemblyPath);
			}
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