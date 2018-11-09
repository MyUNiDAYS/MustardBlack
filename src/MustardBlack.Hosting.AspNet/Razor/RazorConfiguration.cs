using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MustardBlack.ViewEngines.Razor;

namespace MustardBlack.Hosting.AspNet.Razor
{
	public class RazorConfiguration : IRazorConfiguration
	{
		IEnumerable<string> namespaces;
		IEnumerable<Type> tagHelpers;
		public string OutPath => Path.GetTempPath();

		public ICompilerSettings CompilerSettings { get; protected set;  }

		public RazorConfiguration(IFileSystem fileSystem)
		{
			this.namespaces = Enumerable.Empty<string>();
			this.tagHelpers = Enumerable.Empty<Type>();

			if (!fileSystem.Exists("~/web.config"))
				return;

			fileSystem.Read("~/web.config", reader =>
			{
				var doc = new XmlDocument();
				doc.Load(reader);
				// Like this because if you load using ConfigurationManager, you need to reference 1000 MS assemblies.
				var namespaceNodes = doc.DocumentElement.SelectNodes("system.web.webPages.razor/pages/namespaces/add");
				this.namespaces = namespaceNodes?.Cast<XmlNode>().Select(e => e.Attributes["namespace"].Value).ToArray() ?? Enumerable.Empty<string>();

				var tagHelperNodes = doc.DocumentElement.SelectNodes("system.web.webPages.razor/pages/taghelpers/add");
				this.tagHelpers = tagHelperNodes?.Cast<XmlNode>().Select(e => Type.GetType(e.Attributes["type"].Value)).Where(t => t != null).ToArray() ?? Enumerable.Empty<Type>();
				return true;
			});

			this.CompilerSettings = new CompilerSettings(@"bin\roslyn\csc.exe");
		}

		/// <summary>
		/// Gets the default namespaces to be included in the generated code.
		/// </summary>
		public IEnumerable<string> GetDefaultNamespaces()
		{
			return this.namespaces;
		}

		public IEnumerable<Type> GetDefaultTagHelpers()
		{
			return this.tagHelpers;
		}

		public Assembly GetApplicationAssembly()
		{
			// Try the EntryAssembly, this doesn't work for ASP.NET apps
			var ass = Assembly.GetEntryAssembly();

			// Look for web application assembly
			var ctx = HttpContext.Current;
			if (ctx != null)
				ass = GetWebApplicationAssembly(ctx);

			// Fallback to executing assembly
			return ass ?? Assembly.GetExecutingAssembly();
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
	}
}
