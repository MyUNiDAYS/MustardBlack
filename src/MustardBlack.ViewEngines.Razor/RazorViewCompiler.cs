using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MustardBlack.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Razor;

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
			GetAssemblyPath(typeof(IHtmlString).Assembly)
		};

		readonly RazorTemplateEngine razorTemplateEngine;
		readonly RazorSourceDocument[] defaultImports;
		List<string> referenceAssemblies;
		readonly CSharpCodeProvider codeProvider;

		public RazorViewCompiler(IFileSystem fileSystem, IRazorConfiguration razorConfiguration)
		{
			this.fileSystem = fileSystem;
		    this.razorConfiguration = razorConfiguration;
			this.razorTemplateEngine = BuildRazorTemplateEngine(RazorProjectFileSystem.Create(this.fileSystem.GetFullPath("~/")));
			this.defaultImports = new[] { RazorSourceDocument.ReadFrom(new DefaultDirectivesProjectItem(this.razorConfiguration.GetDefaultNamespaces())) };
			this.GetReferenceAssemblies();

			this.codeProvider = new CSharpCodeProvider();
		}

		void GetReferenceAssemblies()
		{
			var assemblies = new List<string>();
			var appAssembly = GetApplicationAssembly();
			assemblies.AddRange(this.defaultAssemblies);
			assemblies.Add(GetAssemblyPath(appAssembly)); // current app
			assemblies.AddRange(appAssembly.GetReferencedAssemblies().Select(GetAssemblyPath)); // assemblies referenced by current app
			assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(GetAssemblyPath)); // loaded assemblies (superset of above line?)
			
			var assemblyNames = this.razorConfiguration.GetAssemblyNames();
			// TODO: cant just load here, use assembly repos to check uniqueness
			assemblies.AddRange(assemblyNames.Select(Assembly.Load).Select(GetAssemblyPath));
			this.referenceAssemblies = assemblies = assemblies.Distinct(p => Path.GetFileName(p).ToLowerInvariant()).ToList();
		}

		static RazorTemplateEngine BuildRazorTemplateEngine(RazorProjectFileSystem fileSystem)
		{
			var razorConfiguration = Microsoft.AspNetCore.Razor.Language.RazorConfiguration.Default;
			var razorProjectEngine = RazorProjectEngine.Create(razorConfiguration, fileSystem, builder =>
			{
				FunctionsDirective.Register(builder);
				InheritsDirective.Register(builder);
				SectionDirective.Register(builder);

				var metadataReferences = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => MetadataReference.CreateFromFile(a.Location)).ToArray();

				builder.Features.Add(new DefaultMetadataReferenceFeature { References = metadataReferences });
				builder.Features.Add(new CompilationTagHelperFeature());
				builder.Features.Add(new DefaultTagHelperDescriptorProvider());
				builder.Features.Add(new ViewComponentTagHelperDescriptorProvider());
				builder.Features.Add(new DocumentClassifierPass());
				builder.Features.Add(new ViewComponentTagHelperPass());
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
			var ctx = HttpContext.Current;
			if (ctx != null)
				ass = GetWebApplicationAssembly(ctx);

			// Fallback to executing assembly
			return ass ?? (Assembly.GetExecutingAssembly());
		}
		
		static Assembly GetWebApplicationAssembly(HttpContext context)
		{
			object app = context.ApplicationInstance;
			if (app == null) return null;

			var type = app.GetType();
			// TODO: suspect "ASP" is no longer real/correct
			while (type != null && type != typeof(object) && type.Namespace == "ASP")
				type = type.BaseType;

			return type.Assembly;
		}

		public Type CompileFile(RazorViewCompilationData view)
		{
			var razorCSharpDocument = GenerateCSharp(view);
			return CompileCSharp(view, razorCSharpDocument);
		}

		RazorCSharpDocument GenerateCSharp(RazorViewCompilationData view)
		{
			var sourceDocument = RazorSourceDocument.Create(view.ViewContents, view.FilePath, Encoding.UTF8);
			var codeDocument = RazorCodeDocument.Create(sourceDocument, this.defaultImports);

			codeDocument.Items.Add("ViewCompilationData", view);
			var razorCSharpDocument = this.razorTemplateEngine.GenerateCode(codeDocument);
			return razorCSharpDocument;
		}

		Type CompileCSharp(RazorViewCompilationData view, RazorCSharpDocument razorCSharpDocument)
		{
			var compilerParameters = new CompilerParameters(this.referenceAssemblies.ToArray());
			compilerParameters.IncludeDebugInformation = true;
			compilerParameters.TempFiles.KeepFiles = false;
			var compilationResults = this.codeProvider.CompileAssemblyFromSource(compilerParameters, razorCSharpDocument.GeneratedCode);
			if (compilationResults.Errors.HasErrors)
			{
				var errors = compilationResults.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).Select(error => $"[{error.ErrorNumber}] Line: {error.Line} Column: {error.Column} - {error.ErrorText}").Aggregate((s1, s2) => s1 + "\n" + s2);
				throw new ViewRenderException("Failed to compile view `" + view.FilePath + "`: " + errors, razorCSharpDocument.GeneratedCode);
			}

			var type = compilationResults.CompiledAssembly.GetType(view.Namespace + "." + view.ClassName);
			if (type == null)
				throw new ViewRenderException($"Could not find type `{view.Namespace + "." + view.ClassName}` in assembly `{compilationResults.CompiledAssembly.FullName}`");

			return type;
		}

		public void CompileAndMergeFiles(IEnumerable<RazorViewCompilationData> viewCompilationDetails, string outputAssemblyName)
		{
//			var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
//
//			foreach (var @namespace in this.razorConfiguration.GetDefaultNamespaces().Union(this.defaultNamespaces))
//				host.NamespaceImports.Add(@namespace);
//
//			var engine = new RazorTemplateEngine(host);
//
//			var razorResults = viewCompilationDetails
//				.Select(v =>
//				{
//					var stringReader = new StringReader(v.ViewContents);
//					return engine.GenerateCode(stringReader, v.Name, "", v.Name);
//				})
//				.Select(r => r.GeneratedCode);
//
//			var assemblies = new List<string>();
//			var appAssembly = GetApplicationAssembly();
//
//			assemblies.AddRange(this.defaultAssemblies);
//			assemblies.AddRange(appAssembly.GetReferencedAssemblies().Select(GetAssemblyPath)); // assemblies referenced by current app
//			assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(GetAssemblyPath)); // loaded assemblies (superset of above line?)
//			//.AddRange(AssemblyRepository.GetApplicationAssemblies().Select(GetAssemblyPath)); // assemblies in app's folder
//
//			// assemblies named by configuration
//			var configuredAssemblyNames = this.razorConfiguration.GetAssemblyNames();
//			// TODO: cant just load here, use assembly repos to check uniqueness
//			assemblies.AddRange(configuredAssemblyNames.Select(Assembly.Load).Select(GetAssemblyPath));
//
//			var assemblyNames = assemblies.Distinct(p => Path.GetFileName(p).ToLowerInvariant()).ToArray();
//
//			var outputAssemblyPath = Path.Combine(this.razorConfiguration.OutPath, outputAssemblyName + ".Views.Razor.dll");
//
//			var compilerParameters = new CompilerParameters(assemblyNames, outputAssemblyPath, true) { TempFiles = { KeepFiles = false } };
//			var codeProvider = new CSharpCodeProvider();
//			var compilationResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.ToArray());
//
//			if (compilationResults.Errors.HasErrors)
//			{
//				var errors = compilationResults.Errors
//					.OfType<CompilerError>()
//					.Where(ce => !ce.IsWarning)
//					.Select(error => $"[{error.ErrorNumber}] {error.FileName}: Line: {error.Line} Column: {error.Column} - {error.ErrorText}")
//					.Aggregate((s1, s2) => s1 + "\n" + s2);
//				//TODO: Format Errors nicely
//				throw new ViewRenderException("Failed to compile dll `" + outputAssemblyPath + "`: " + errors);
//			}
//
//			var assembly = Assembly.LoadFrom(outputAssemblyPath);
//			if (assembly == null)
//				throw new ViewRenderException("Error loading template assembly");
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
	        var safe = input.Replace('.', '_');
	        if (safe[0] >= '0' && safe[0] <= '9')
	            safe = '_' + safe;

	        return safe;
	    }
	}
}