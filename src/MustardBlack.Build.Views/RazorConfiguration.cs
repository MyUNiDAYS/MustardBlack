using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MustardBlack.ViewEngines.Razor;

namespace UD.Build.Views
{
	public sealed class RazorConfiguration : IRazorConfiguration
	{
		readonly IEnumerable<string> namespaces;

		/// <summary>
		/// Initializes a new instance of the <see cref="RazorConfiguration"/> class.
		/// </summary>
		public RazorConfiguration(string webconfigPath, string outPath)
		{
			this.OutPath = outPath;
			using (var reader = new StreamReader(webconfigPath))
			{
				var doc = new XmlDocument();
				doc.Load(reader);
				// Like this because if you load using ConfigurationManager, you need to reference 1000 MS assemblies.
				var xmlNodeList = doc.DocumentElement.SelectNodes("system.web.webPages.razor/pages/namespaces/add").Cast<XmlNode>();
				this.namespaces = xmlNodeList.Select(e => e.Attributes["namespace"].Value).ToArray();
			}
		}

		/// <summary>
		/// Gets the assembly names to include in the generated assembly.
		/// </summary>
		public IEnumerable<string> GetAssemblyNames()
		{
			return new string[0];
		}

		/// <summary>
		/// Gets the default namespaces to be included in the generated code.
		/// </summary>
		public IEnumerable<string> GetDefaultNamespaces()
		{
			return this.namespaces;
		}
		
		public string OutPath { get; private set; }
	}
}
