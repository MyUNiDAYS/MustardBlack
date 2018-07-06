using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MustardBlack.Assets;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using MustardBlack.ViewEngines.Razor;

namespace MustardBlack.Build.Views
{
	static class Compiler
	{
		public static void Compile(string inPath, string outPath, string assemblyName)
		{
			var binPath = Path.Combine(inPath, "bin");
			var webConfigPath = Path.Combine(inPath, "web.config");

			AssemblyRepository.Initialize();
			AssemblyRepository.LoadAssembliesFromPath(binPath);

			var razorConfiguration = new RazorConfiguration(webConfigPath, outPath);
			var cssPreprocessor = new LessCssPreprocessor();
			var fileSystem = new BasicFileSystem(inPath);
			var razorViewCompiler = new AssetEnrichedRazorViewCompiler(razorConfiguration, new YuiJavascriptCompressor(), cssPreprocessor, fileSystem, new AssetLoader(fileSystem));

			var views = Directory.GetFiles(inPath, "*.cshtml", SearchOption.AllDirectories).ToList();
			if (!views.Any())
			{
				Console.WriteLine($"No views found in path '{inPath}'");
				return;
			}
			
			// Find all primary (non-component) view paths which need compiling
			views = views.Select(v =>
			{
				var path = Path.GetDirectoryName(v);
				var name = Path.GetFileNameWithoutExtension(v);
				var ext = Path.GetExtension(v);
				if (name.IndexOf(".", StringComparison.Ordinal) > -1)
					return path + "\\" + name.Substring(0, name.IndexOf(".", StringComparison.Ordinal)) + ext;
				return v;
			}).Distinct().ToList();

			var viewData = new List<RazorViewCompilationData>(views.Count);

			foreach (var view in views)
			{
				var viewPaths = razorViewCompiler.GetViewComponentPaths(view, ".cshtml");
				var jsPaths = razorViewCompiler.GetViewComponentPaths(view, ".js");
				var cssPaths = razorViewCompiler.GetViewComponentPaths(view, ".less").Union(razorViewCompiler.GetViewComponentPaths(view, ".css"));

				var viewVirtualPath = view.Substring(inPath.Length + 1);
				var name = RazorViewCompiler.GetTypeName(viewVirtualPath);

				var areaStartIndex = viewVirtualPath.IndexOf("areas\\", StringComparison.InvariantCultureIgnoreCase) + 6;
				var areaName = viewVirtualPath.Substring(areaStartIndex, viewVirtualPath.IndexOf("\\", areaStartIndex) - areaStartIndex);

				var viewContents = new StringBuilder();

				foreach (var viewFile in viewPaths)
				{
					using (var reader = new StreamReader(new FileStream(viewFile, FileMode.Open, FileAccess.Read)))
					{
						string line;
						while ((line = reader.ReadLine()) != null)
						{
							var trimmedLine = line.Trim();
							if (!string.IsNullOrEmpty(trimmedLine))
							{
								if (trimmedLine.IndexOf("@inherits") == -1 && trimmedLine.EndsWith(">"))
									viewContents.Append(trimmedLine);
								else
									viewContents.AppendLine(trimmedLine);
							}
						}
					}
				}

				if (jsPaths.Any())
				{
					var jsBuilder = new StringBuilder();
					foreach (var jsFile in jsPaths)
						jsBuilder.AppendLine(File.ReadAllText(jsFile));

					viewContents.Insert(0, razorViewCompiler.PrepareJsForRazorCompilation(jsBuilder.ToString(), true));
				}

				if (cssPaths.Any())
				{
					var cssBuilder = new StringBuilder();
					foreach (var cssFile in cssPaths)
						cssBuilder.AppendLine(File.ReadAllText(cssFile));

					viewContents.Insert(0, razorViewCompiler.PrepareCssForRazorCompilation(cssBuilder.ToString(), areaName));
				}

				viewData.Add(new RazorViewCompilationData
				{
					Name = name,
					ViewContents = viewContents.ToString()
				});
			}

			razorViewCompiler.CompileAndMergeFiles(viewData, assemblyName);
		}
	}
}
