using System;
using System.IO;
using MustardBlack.Assets.Babel;
using MustardBlack.Assets.Javascript;
using MustardBlack.Assets.YuiCompressor;

namespace MustardBlack.ViewEngines.Razor.Build
{
	static class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length < 3)
			{
				Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} INPUT_PATH OUTPUT_PATH ASSEMBLY_NAME");
				Environment.Exit(-1);
			}

			var inPath = Path.GetFullPath(args[0]);
			var outPath = Path.GetFullPath(args[1]);
			var jsPreprocessor = Path.GetFullPath(args[2]);
			var assemblyName = args[3];

			IJavascriptPreprocessor javascriptPreprocessor;
			if(jsPreprocessor == "babel")
				javascriptPreprocessor = new BabelJavascriptPreprocessor();
			else
				javascriptPreprocessor = new YuiJavascriptPreprocessor();


			try
			{
				Compiler.Compile(inPath, outPath, assemblyName, javascriptPreprocessor);
			}
			catch (Exception e)
			{
				while (e != null)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					Console.WriteLine();
					e = e.InnerException;
				}

				Environment.Exit(-1);
			}
		}
	}
}
