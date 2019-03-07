using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MustardBlack.Assets.Css;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class DebugRazorViewLocator : IViewLocator
	{
		readonly AssetEnrichedRazorViewCompiler compiler;
		readonly IFileSystem fileSystem;
		readonly IEnumerable<ICssPreprocessor> cssPreprocessors;
		readonly IDictionary<string, CachedItem> cache;

		public DebugRazorViewLocator(AssetEnrichedRazorViewCompiler razorViewCompiler, IFileSystem fileSystem, IEnumerable<ICssPreprocessor> cssPreprocessors)
		{
			this.compiler = razorViewCompiler;
			this.fileSystem = fileSystem;
			this.cssPreprocessors = cssPreprocessors;
			this.cache = new Dictionary<string, CachedItem>();
		}

		public Type Locate(string viewPath)
		{
			var fullViewPath = this.fileSystem.GetFullPath(viewPath);
			var directoryPath = fullViewPath.Substring(0, fullViewPath.LastIndexOf('\\'));

			var cssPreprocessor = this.GetCssPreprocessor(directoryPath);

			var fullViewPaths = this.compiler.GetViewComponentPaths(fullViewPath, new Regex(@"\.cshtml$"));
			var fullJsPaths = this.compiler.GetViewComponentPaths(fullViewPath, new Regex(@"\.js$")).Where(p => !p.EndsWith(".test.js")).ToArray();
			var fullLessPaths = cssPreprocessor == null ? 
				new string[0] :
				this.compiler.GetViewComponentPaths(fullViewPath, cssPreprocessor.FileMatch);

			if (!fullViewPaths.Any())
				return null;

			var sourceCodeLastModified = this.GetLastModified(fullViewPaths, fullJsPaths, fullLessPaths);

			if (this.cache.ContainsKey(viewPath) && sourceCodeLastModified != this.cache[viewPath].LastModified)
				this.cache.Remove(viewPath);

			if (!this.cache.ContainsKey(viewPath))
			{
				lock (this.cache)
				{
					if (!this.cache.ContainsKey(viewPath))
					{
						var compiled = this.CompileView(viewPath, fullJsPaths, fullLessPaths, fullViewPaths, fullViewPath);
						this.cache.Add(viewPath, new CachedItem(compiled, sourceCodeLastModified));
					}
				}
			}

			return this.cache[viewPath].ViewType;
		}

		Type CompileView(string viewPath, IEnumerable<string> fullJsPaths, IEnumerable<string> fullLessPaths, IEnumerable<string> fullViewPaths, string fullViewPath)
		{
			var builder = new StringBuilder();

			this.AppendHtml(fullViewPaths, builder);

			this.AppendJavascript(fullJsPaths, builder);

			// assumes viewPath is of the form "~/areas/{areaName}/path.cshtml
			var areaName = viewPath.Substring(8, viewPath.IndexOf("/", 8) - 8);
			this.AppendCss(fullLessPaths, areaName, builder);
			
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

		DateTime GetLastModified(IEnumerable<string> fullViewPaths, IEnumerable<string> fullJsPaths, IEnumerable<string> fullLessPaths)
		{
			var maxLastModified = DateTime.MinValue;

			var allFilePaths = fullViewPaths.Union(fullJsPaths).Union(fullLessPaths);
			foreach (var file in allFilePaths)
			{
				var fileLastModified = this.fileSystem.GetLastWriteTime(file);
				if (fileLastModified > maxLastModified)
					maxLastModified = fileLastModified;
			}

			return maxLastModified;
		}

		ICssPreprocessor GetCssPreprocessor(string viewPath)
		{
			var styles = Directory.
				EnumerateFiles(viewPath)
				.Where(file => file.ToLower().EndsWith("scss") || file.ToLower().EndsWith("less") || file.ToLower().EndsWith("css")).ToList();

			if (!styles.Any())
				return null;

			return this.cssPreprocessors.First(x => x.FileMatch.IsMatch(styles.First()));
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

		void AppendCss(IEnumerable<string> fullLessPaths, string areaName, StringBuilder builder)
		{
			if (!fullLessPaths.Any())
				return;
			
			var lessBuilder = new StringBuilder();
			foreach (var lessFile in fullLessPaths)
				this.fileSystem.Read(lessFile, reader => lessBuilder.AppendLine(reader.ReadToEnd()));

			var css = this.compiler.PrepareCssForRazorCompilation(lessBuilder.ToString(), areaName);
			if(!string.IsNullOrWhiteSpace(css))
				builder.Insert(0, css);
		}

		void AppendJavascript(IEnumerable<string> fullJsPaths, StringBuilder builder)
		{
			if (!fullJsPaths.Any())
				return;

			var jsBuilder = new StringBuilder();
			foreach (var jsFile in fullJsPaths)
				this.fileSystem.Read(jsFile, reader => jsBuilder.AppendLine(reader.ReadToEnd()));
			
			builder.Insert(0, this.compiler.PrepareJsForRazorCompilation(jsBuilder.ToString(), false));
		}
	}
}
