using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines.Razor
{
	public class RazorConfiguration : IRazorConfiguration
	{
		IEnumerable<string> namespaces;
		IEnumerable<Type> tagHelpers;
		public string OutPath => Path.GetTempPath();

		public ICompilerSettings CompilerSettings { get; protected set;  }

		public RazorConfiguration(IFileSystem fileSystem)
		{
			if (!fileSystem.Exists("~/web.config"))
			{
				this.namespaces = new string[0];
                return;
			} 

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
	}
}
