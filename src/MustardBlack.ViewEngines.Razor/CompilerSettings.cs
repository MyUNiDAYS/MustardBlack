using System;
using System.IO;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class CompilerSettings : ICompilerSettings
	{
		public string CompilerFullPath { get; }
		public int CompilerServerTimeToLive { get; }

		public CompilerSettings(string path = null)
		{
			this.CompilerFullPath = path ?? GetCompilerFullPath(@"bin\roslyn\csc.exe");
			this.CompilerServerTimeToLive = 60;
		}

		static string GetCompilerFullPath(string relativePath)
		{
			string compilerFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
			return compilerFullPath;
		}
	}
}