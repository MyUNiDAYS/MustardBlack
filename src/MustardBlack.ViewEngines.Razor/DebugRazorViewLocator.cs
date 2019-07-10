using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MustardBlack.Assets;
using MustardBlack.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class DebugRazorViewLocator : IViewLocator
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		readonly AssetEnrichedRazorViewCompiler compiler;
		readonly IFileSystem fileSystem;
		readonly IDictionary<string, CachedItem> cache;

		public DebugRazorViewLocator(AssetEnrichedRazorViewCompiler razorViewCompiler, IFileSystem fileSystem)
		{
			this.compiler = razorViewCompiler;
			this.fileSystem = fileSystem;
			this.cache = new Dictionary<string, CachedItem>();
		}

		public Type Locate(string viewPath)
		{
			// If you need to modify this code, you must also edit its counterpart in MustardBlack.Build.Views

			var fullViewPath = this.fileSystem.GetFullPath(viewPath);

			var fullViewPaths = this.compiler.GetViewComponentPaths(fullViewPath, ".cshtml");
			var fullJsPaths = this.compiler.GetViewComponentPaths(fullViewPath, ".js").Where(p => !p.EndsWith(".test.js")).ToArray();
			var fullCssPaths = this.compiler
					.GetViewComponentPaths(fullViewPath, ".less")
					.Union(this.compiler.GetViewComponentPaths(fullViewPath, ".scss"))
					.Union(this.compiler.GetViewComponentPaths(fullViewPath, ".css"))
					.ToArray();

			if (!fullViewPaths.Any())
				return null;

			var sourceCodeLastModified = this.GetLastModified(fullViewPaths, fullJsPaths, fullCssPaths);

			if (this.cache.ContainsKey(viewPath) && sourceCodeLastModified != this.cache[viewPath].LastModified)
				this.cache.Remove(viewPath);

			if (!this.cache.ContainsKey(viewPath))
			{
				lock (this.cache)
				{
					if (!this.cache.ContainsKey(viewPath))
					{
						var compiled = this.CompileView(viewPath, fullJsPaths, fullCssPaths, fullViewPaths);
						this.cache.Add(viewPath, new CachedItem(compiled, sourceCodeLastModified));
					}
				}
			}

			return this.cache[viewPath].ViewType;
		}

		Type CompileView(string viewPath, IEnumerable<string> fullJsPaths, IEnumerable<string> fullCssPaths, IEnumerable<string> fullViewPaths)
		{
			log.Debug($"CompileView {viewPath} - JS Files: {JsonConvert.SerializeObject(fullJsPaths)} - CSS Files: {JsonConvert.SerializeObject(fullCssPaths)} - ViewFiles: {JsonConvert.SerializeObject(fullViewPaths)}");

			var builder = new StringBuilder();

			this.AppendHtml(fullViewPaths, builder);

			this.AppendJavascript(fullJsPaths, builder);

			// assumes viewPath is of the form "~/areas/{areaName}/path.cshtml
			var areaName = viewPath.Substring(8, viewPath.IndexOf("/", 8) - 8);
			this.AppendCss(fullCssPaths, areaName, builder);
			
			var viewCompilationData = new RazorViewCompilationData
			{
				Namespace = Path.GetDirectoryName(viewPath.Substring(2)).Replace("\\", ".").Replace("/", "."),
				ClassName = RazorViewCompiler.GetSafeClassName(Path.GetFileName(viewPath)),
				FilePath = viewPath,
				ViewContents = builder.ToString()
			};

			var compiled = this.compiler.CompileFile(viewCompilationData);
			return compiled;
		}

		DateTime GetLastModified(IEnumerable<string> fullViewPaths, IEnumerable<string> fullJsPaths, IEnumerable<string> fullCssPaths)
		{
			var maxLastModified = DateTime.MinValue;

			var allFilePaths = fullViewPaths.Union(fullJsPaths).Union(fullCssPaths);
			foreach (var file in allFilePaths)
			{
				var fileLastModified = this.fileSystem.GetLastWriteTime(file);
				if (fileLastModified > maxLastModified)
					maxLastModified = fileLastModified;
			}

			return maxLastModified;
		}

		void AppendHtml(IEnumerable<string> fullViewPaths, StringBuilder builder)
		{
			foreach (var viewFile in fullViewPaths)
			{
				this.fileSystem.Read(viewFile, reader =>
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						var trimmedLine = line.Trim();
						if (!string.IsNullOrEmpty(trimmedLine))
						{
							if (trimmedLine.IndexOf("@section") == 0)
								builder.AppendLine().AppendLine(trimmedLine);
							else if (trimmedLine.IndexOf("@inherits") == 0)
								builder.AppendLine().AppendLine(trimmedLine);
							else
								builder.AppendLine(trimmedLine);
						}
					}
					return builder;
				});
			}
		}

		void AppendCss(IEnumerable<string> fullCssPaths, string areaName, StringBuilder builder)
		{
			if (!fullCssPaths.Any())
				return;
			
			var cssBuilder = new StringBuilder();
			foreach (var cssFile in fullCssPaths)
				this.fileSystem.Read(cssFile, reader => cssBuilder.AppendLine(reader.ReadToEnd()));

			var css = this.compiler.PrepareCssForRazorCompilation(cssBuilder.ToString(), areaName);
			if(!string.IsNullOrWhiteSpace(css))
				builder.Insert(0, css);
		}

		void AppendJavascript(IEnumerable<string> fullJsPaths, StringBuilder builder)
		{
			if (!fullJsPaths.Any())
				return;

			var root = this.fileSystem.GetFullPath("~/");

			var assets = new List<AssetContent>();
			foreach (var jsPath in fullJsPaths)
			{
				this.fileSystem.Read(jsPath, reader =>
				{
					var relativePath = jsPath.Substring(root.Length - 1).Replace('\\', '/');
					assets.Add(new AssetContent(relativePath, reader.ReadToEnd()));
					return 0;
				});
			}

			builder.Insert(0, this.compiler.PrepareJsForRazorCompilation(assets));
		}
	}
}
