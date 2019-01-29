using System.Collections.Generic;
using MustardBlack.Assets;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines.Razor
{
	public class AssetEnrichedRazorViewCompiler : RazorViewCompiler
	{
		readonly IJavascriptCompressor jsCompressor;
		readonly IEnumerable<ICssPreprocessor> cssPreprocessors;
		readonly IAssetLoader assetLoader;

		public AssetEnrichedRazorViewCompiler(IJavascriptCompressor jsCompressor, IEnumerable<ICssPreprocessor> cssPreprocessors, IFileSystem fileSystem, IAssetLoader assetLoader, IRazorConfiguration razorConfiguration) : base(fileSystem, razorConfiguration)
		{
			this.jsCompressor = jsCompressor;
			this.cssPreprocessors = cssPreprocessors;
			this.assetLoader = assetLoader;
		}
		
		public string PrepareJsForRazorCompilation(string input, bool compress)
		{
			if (string.IsNullOrEmpty(input))
				return null;

			input = input.Replace("@", "@@");

			if (compress)
			{
				var compressed = this.jsCompressor.Compress(input);
				return @"<script type=""text/javascript"">window.currentPageScript=function(){" + compressed + @"}</script>";
			}

			// There are certain ways "<" inside <text></text> can confuse the Razor parser.
			// This seems to only be an issue when not compressed, although that is pure chance
			// Example parser confusing syntax: `@{<text>if(1 < 2) f('=');</text>}`
			// Hopefully this works around them all
			var javascript = input.Replace("<", "@(_lessThanSymbol)");
			return "<script type=\"text/javascript\">\n@{var _lessThanSymbol = new Microsoft.AspNetCore.Html.HtmlString(\"<\");}window.currentPageScript=function(){\n" + javascript + "\n}\n</script>";
		}
		
		public string PrepareCssForRazorCompilation(string input, string areaName)
		{
			if (string.IsNullOrEmpty(input))
				return null;
			
			foreach(var cssPreprocessor in this.cssPreprocessors)
			{
				var styles = this.assetLoader.GetAsset("~/areas/" + areaName + "/", cssPreprocessor.FileMatch);

				if (string.IsNullOrWhiteSpace(styles))
					continue;

				var cssResult = cssPreprocessor.Process(input, styles);

				var css = cssResult.Status == AssetProcessingResult.CompilationStatus.Success ? cssResult.Result : cssResult.Message;
				css = css.Replace("@", "@@");

				return "<style type=\"text/css\">" + css + "</style>";
			}

			return null;
		}
	}
}
