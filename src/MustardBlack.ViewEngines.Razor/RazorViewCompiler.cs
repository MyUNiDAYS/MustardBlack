using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MustardBlack.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace MustardBlack.ViewEngines.Razor
{
	public class RazorViewCompiler
	{
		//protected readonly IRazorConfiguration razorConfiguration;
		protected readonly IFileSystem fileSystem;
	    readonly IRazorConfiguration razorConfiguration;

	    readonly string[] defaultAssemblies =
		{
			GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite)),
			GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder)),
			GetAssemblyPath(typeof(IHtmlString))
		};

		readonly string[] defaultNamespaces =
		{
			"Microsoft.CSharp.RuntimeBinder",
			"System",
			"System.Linq",
			"MustardBlack.ViewEngines"
		};

		RazorProjectFileSystem razorProjectFileSystem;

		public RazorViewCompiler(IFileSystem fileSystem, IRazorConfiguration razorConfiguration)
		{
			this.fileSystem = fileSystem;
		    this.razorConfiguration = razorConfiguration;
		    this.razorProjectFileSystem = RazorProjectFileSystem.Create(this.fileSystem.GetFullPath("~/"));
		}

		static string GetAssemblyPath(Type type)
		{
			return GetAssemblyPath(type.Assembly);
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

		const string AspNetNamespace = "ASP";

		static Assembly GetWebApplicationAssembly(HttpContext context)
		{
			object app = context.ApplicationInstance;
			if (app == null) return null;

			var type = app.GetType();
			while (type != null && type != typeof(object) && type.Namespace == AspNetNamespace)
				type = type.BaseType;

			return type.Assembly;
		}

		public Type CompileFile(RazorViewCompilationData view, IEnumerable<Assembly> assembliesToReference, bool includeDebugInformation = false, string debugFilePath = null)
		{
			var razorCSharpDocument = GenerateCSharp(view);
			return CompileCSharp(view, assembliesToReference, razorCSharpDocument);
		}

		RazorCSharpDocument GenerateCSharp(RazorViewCompilationData view)
		{
			var razorConfiguration = Microsoft.AspNetCore.Razor.Language.RazorConfiguration.Default;
			var razorProjectEngine = RazorProjectEngine.Create(razorConfiguration, razorProjectFileSystem, builder =>
			{
				builder.SetBaseType("MustardBlack.ViewEngines.Razor.RazorViewPage");

				builder.SetNamespace(view.Namespace);
				builder.ConfigureClass((doc, node) => node.ClassName = view.ClassName);

				FunctionsDirective.Register(builder);
				InheritsDirective.Register(builder);
				SectionDirective.Register(builder);
			});

			var templateEngine = new RazorTemplateEngine(razorProjectEngine.Engine, razorProjectFileSystem);

			var sourceDocument = RazorSourceDocument.Create(view.ViewContents, view.FilePath, Encoding.UTF8);
			var codeDocument = RazorCodeDocument.Create(sourceDocument);
			var razorCSharpDocument = templateEngine.GenerateCode(codeDocument);
			return razorCSharpDocument;
		}

		Type CompileCSharp(RazorViewCompilationData view, IEnumerable<Assembly> assembliesToReference, RazorCSharpDocument razorCSharpDocument)
		{
			var assemblies = new List<string>();
			var appAssembly = GetApplicationAssembly();
			assemblies.AddRange(this.defaultAssemblies);
			assemblies.Add(GetAssemblyPath(appAssembly)); // current app
			assemblies.AddRange(appAssembly.GetReferencedAssemblies().Select(GetAssemblyPath)); // assemblies referenced by current app
			assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(GetAssemblyPath)); // loaded assemblies (superset of above line?)
//			assemblies.AddRange(AssemblyRepository.GetApplicationAssemblies().Select(GetAssemblyPath)); // assemblies in app's folder

			// assemblies named by configuration
			var assemblyNames = this.razorConfiguration.GetAssemblyNames();
			// TODO: cant just load here, use assembly repos to check uniqueness
			assemblies.AddRange(assemblyNames.Select(Assembly.Load).Select(GetAssemblyPath));
			assemblies.AddRange(assembliesToReference.Select(GetAssemblyPath));
			assemblies = assemblies.Distinct(p => Path.GetFileName(p).ToLowerInvariant()).ToList();
			var compilerParameters = new CompilerParameters(assemblies.ToArray());
			compilerParameters.IncludeDebugInformation = true;
			compilerParameters.TempFiles.KeepFiles = false;
			var codeProvider = new CSharpCodeProvider();

			var compilationResults = codeProvider.CompileAssemblyFromSource(compilerParameters, razorCSharpDocument.GeneratedCode);
			if (compilationResults.Errors.HasErrors)
			{
				var errors = compilationResults.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).Select(error => $"[{error.ErrorNumber}] Line: {error.Line} Column: {error.Column} - {error.ErrorText}").Aggregate((s1, s2) => s1 + "\n" + s2);
				//TODO: Format Errors nicely
				throw new ViewRenderException("Failed to compile view `" + view.FilePath + "`: " + errors);
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
	}
}