using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

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

		ICompilerSettings CompilerSettings { get; }
		Assembly GetApplicationAssembly();
	}
}