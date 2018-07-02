using System;
using System.IO;

namespace UD.Build.Views
{
	static class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 3)
			{
				Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} INPUT_PATH OUTPUT_PATH ASSEMBLY_NAME");
				Environment.Exit(-1);
			}

			var inPath = Path.GetFullPath(args[0]);
			var outPath = Path.GetFullPath(args[1]);
			var assemblyName = args[2];

			try
			{
				Compiler.Compile(inPath, outPath, assemblyName);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Environment.Exit(-1);
			}
		}
	}
}
