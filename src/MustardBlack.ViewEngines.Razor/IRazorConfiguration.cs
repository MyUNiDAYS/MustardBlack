using System;
using System.Collections.Generic;
using System.Reflection;

namespace MustardBlack.ViewEngines.Razor
{
	public interface IRazorConfiguration
	{
		/// <summary>
		/// Gets the default namespaces.
		/// </summary>
		IEnumerable<string> GetDefaultNamespaces();

		/// <summary>
		/// The full path to place compiled assemblies
		/// </summary>
		string OutPath { get; }

		IEnumerable<Type> GetDefaultTagHelpers();

		Assembly GetApplicationAssembly();
	}
}