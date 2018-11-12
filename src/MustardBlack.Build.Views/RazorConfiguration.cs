using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using MustardBlack.ViewEngines.Razor;

namespace MustardBlack.Build.Views
{
	public sealed class RazorConfiguration : IRazorConfiguration
	{
		readonly IEnumerable<string> namespaces;
		readonly IEnumerable<Type> tagHelpers;
		public string OutPath { get; }
		
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

				xmlNodeList = doc.DocumentElement.SelectNodes("system.web.webPages.razor/pages/taghelpers/add").Cast<XmlNode>();
				this.tagHelpers = xmlNodeList.Select(e =>
					{
						var typeName = e.Attributes["type"].Value;

						return Type.GetType(typeName, assemblyName =>
						{
							return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName.FullName);
						}, null, true);
					})
					.Where(t => t != null).ToArray();
			}
		}

		public IEnumerable<Type> GetDefaultTagHelpers()
		{
			return this.tagHelpers;
		}

		public Assembly GetApplicationAssembly()
		{
			return Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
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
