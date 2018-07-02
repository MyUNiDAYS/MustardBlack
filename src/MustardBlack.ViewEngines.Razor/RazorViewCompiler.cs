using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MustardBlack.Hosting;
using System.Web.Razor;

namespace MustardBlack.ViewEngines.Razor
{
	public class RazorViewCompiler
	{
		protected readonly IRazorConfiguration razorConfiguration;
		protected readonly IFileSystem fileSystem;

		readonly string[] defaultAssemblies = {
			GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite)),
			GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder)),
			GetAssemblyPath(typeof(IHtmlString))
		};

		readonly string[] defaultNamespaces = {
			"Microsoft.CSharp.RuntimeBinder",
			"System",
			"System.Linq",
			"MustardBlack.ViewEngines"
		};

		public RazorViewCompiler(IRazorConfiguration razorConfiguration, IFileSystem fileSystem)
		{
			this.razorConfiguration = razorConfiguration;
			this.fileSystem = fileSystem;
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
			var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
			
			foreach (var @namespace in this.razorConfiguration.GetDefaultNamespaces().Union(this.defaultNamespaces))
				host.NamespaceImports.Add(@namespace);
			
			var engine = new RazorTemplateEngine(host);

//			host.EnableInstrumentation = includeDebugInformation;
//			host.InstrumentedSourceFilePath = debugFilePath;

			GeneratorResults razorResult;
			using (var textReader = new StringReader(view.ViewContents))
				razorResult = engine.GenerateCode(textReader, view.Name, "", debugFilePath);

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
			var compilationResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

			if (compilationResults.Errors.HasErrors)
			{
				var errors = compilationResults.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).Select(error => String.Format(
					"[{0}] Line: {1} Column: {2} - {3}",
					error.ErrorNumber,
					error.Line,
					error.Column,
					error.ErrorText)).Aggregate((s1, s2) => s1 + "\n" + s2);
				//TODO: Format Errors nicely
				throw new ViewRenderException("Failed to compile view `" + view.Name + "`: " + errors);
			}

			var type = compilationResults.CompiledAssembly.GetType(view.Name);
			if (type == null)
				throw new ViewRenderException($"Could not find type `{view.Name}` in assembly `{compilationResults.CompiledAssembly.FullName}`");
//
//			if (Activator.CreateInstance(type) as RazorViewPage == null)
//				throw new ViewRenderException(string.Format("Could not construct `{0}` or it does not inherit from RazorViewPage", type));

			return type;
		}

		public void CompileAndMergeFiles(IEnumerable<RazorViewCompilationData> viewCompilationDetails, string outputAssemblyName)
		{
			var host = new RazorEngineHost(new CSharpRazorCodeLanguage());

			foreach (var @namespace in this.razorConfiguration.GetDefaultNamespaces().Union(this.defaultNamespaces))
				host.NamespaceImports.Add(@namespace);

			var engine = new RazorTemplateEngine(host);

			var razorResults = viewCompilationDetails
				.Select(v =>
				{
					var stringReader = new StringReader(v.ViewContents);
					return engine.GenerateCode(stringReader, v.Name, "", v.Name);
				})
				.Select(r => r.GeneratedCode);

			var assemblies = new List<string>();
			var appAssembly = GetApplicationAssembly();

			assemblies.AddRange(this.defaultAssemblies);
			assemblies.AddRange(appAssembly.GetReferencedAssemblies().Select(GetAssemblyPath)); // assemblies referenced by current app
			assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(GetAssemblyPath)); // loaded assemblies (superset of above line?)
			//.AddRange(AssemblyRepository.GetApplicationAssemblies().Select(GetAssemblyPath)); // assemblies in app's folder

			// assemblies named by configuration
			var configuredAssemblyNames = this.razorConfiguration.GetAssemblyNames();
			// TODO: cant just load here, use assembly repos to check uniqueness
			assemblies.AddRange(configuredAssemblyNames.Select(Assembly.Load).Select(GetAssemblyPath));

			var assemblyNames = assemblies.Distinct(p => Path.GetFileName(p).ToLowerInvariant()).ToArray();

			var outputAssemblyPath = Path.Combine(this.razorConfiguration.OutPath, outputAssemblyName + ".Views.Razor.dll");

			var compilerParameters = new CompilerParameters(assemblyNames, outputAssemblyPath, true) { TempFiles = { KeepFiles = false } };
			var codeProvider = new CSharpCodeProvider();
			var compilationResults = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.ToArray());

			if (compilationResults.Errors.HasErrors)
			{
				var errors = compilationResults.Errors
					.OfType<CompilerError>()
					.Where(ce => !ce.IsWarning)
					.Select(error => $"[{error.ErrorNumber}] {error.FileName}: Line: {error.Line} Column: {error.Column} - {error.ErrorText}")
					.Aggregate((s1, s2) => s1 + "\n" + s2);
				//TODO: Format Errors nicely
				throw new ViewRenderException("Failed to compile dll `" + outputAssemblyPath + "`: " + errors);
			}

			var assembly = Assembly.LoadFrom(outputAssemblyPath);
			if (assembly == null)
				throw new ViewRenderException("Error loading template assembly");
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

		public static string GetTypeName(string path)
		{
			// TODO: leading numerics arent handled, i think they get a leading underscore prefix
			return path.Replace("\\", "_").Replace("/", "_").Replace(".", "_").Replace("-", "_").ToLowerInvariant();
		}
	} 
}
