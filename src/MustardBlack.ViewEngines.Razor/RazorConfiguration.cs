using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class RazorConfiguration : IRazorConfiguration
	{
		readonly IEnumerable<string> namespaces;

		public RazorConfiguration(IFileSystem fileSystem)
		{
			if (!fileSystem.Exists("~/web.config"))
			{
				this.namespaces = new string[0];
                return;
			} 

			var xmlNodeList = fileSystem.Read("~/web.config", reader =>
			{
				var doc = new XmlDocument();
				doc.Load(reader);
				// Like this because if you load using ConfigurationManager, you need to reference 1000 MS assemblies.
				return doc.DocumentElement.SelectNodes("system.web.webPages.razor/pages/namespaces/add").Cast<XmlNode>(); 
			});
			
			this.namespaces = xmlNodeList.Select(e => e.Attributes["namespace"].Value).ToArray();
		}
		
		/// <summary>
		/// Gets the default namespaces to be included in the generated code.
		/// </summary>
		public IEnumerable<string> GetDefaultNamespaces()
		{
			return this.namespaces;
		}

		public string OutPath => Path.GetTempPath();
	}
}
