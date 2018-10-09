using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using MustardBlack.ViewEngines.Razor;

namespace MustardBlack.Build.Views
{
	public sealed class RazorConfiguration : IRazorConfiguration
	{
		readonly IEnumerable<string> namespaces;
		readonly IEnumerable<Type> tagHelpers;
		public string OutPath { get; }

		public ICompilerSettings CompilerSettings { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RazorConfiguration"/> class.
		/// </summary>
		public RazorConfiguration(string webconfigPath, string outPath)
		{
			this.OutPath = outPath;

			// Nasty hack, The solution to this is to work out how to appease Resharper so that intellisense actually works
			using (var reader = new StreamReader(webconfigPath))
			{
				var doc = new XmlDocument();
				doc.Load(reader);
				// Like this because if you load using ConfigurationManager, you need to reference 1000 MS assemblies.
				var xmlNodeList = doc.DocumentElement.SelectNodes("system.web.webPages.razor/pages/namespaces/add").Cast<XmlNode>();
				this.namespaces = xmlNodeList.Select(e => e.Attributes["namespace"].Value).ToArray();

				this.tagHelpers = doc.DocumentElement.SelectNodes("system.web.webPages.razor/pages/taghelpers/add").Cast<XmlNode>().Select(e => Type.GetType(e.Attributes["type"].Value)).Where(t => t != null).ToArray();
			}

			this.CompilerSettings = new CompilerSettings(@"roslyn\csc.exe");
		}

		public IEnumerable<Type> GetDefaultTagHelpers()
		{
			return this.tagHelpers;
		}

		/// <summary>
		/// Gets the default namespaces to be included in the generated code.
		/// </summary>
		public IEnumerable<string> GetDefaultNamespaces()
		{
			return this.namespaces;
		}
	}
}
