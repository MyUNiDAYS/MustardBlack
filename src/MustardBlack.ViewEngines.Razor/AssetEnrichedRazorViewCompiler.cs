using MustardBlack.Assets;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines.Razor
{
	public class AssetEnrichedRazorViewCompiler : RazorViewCompiler
	{
		readonly IJavascriptCompressor jsCompressor;
		readonly ICssPreprocessor cssPreprocessor;
		readonly IAssetLoader assetLoader;

		public AssetEnrichedRazorViewCompiler(IJavascriptCompressor jsCompressor, ICssPreprocessor cssPreprocessor, IFileSystem fileSystem, IAssetLoader assetLoader) : base(fileSystem)
		{
			this.jsCompressor = jsCompressor;
			this.cssPreprocessor = cssPreprocessor;
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
			return "<script type=\"text/javascript\">\n@{var _lessThanSymbol = new System.Web.HtmlString(\"<\");}window.currentPageScript=function(){\n" + javascript + "\n}\n</script>";
		}
		
		public string PrepareCssForRazorCompilation(string input, string areaName)
		{
			if (string.IsNullOrEmpty(input))
				return null;

			var mixins = this.assetLoader.GetAsset("~/areas/" + areaName + "/", AssetFormat.Css);

			var cssResult = this.cssPreprocessor.Process(input, mixins);

			var css = cssResult.Status == AssetProcessingResult.CompilationStatus.Success ? cssResult.Result : cssResult.Message;
			css = css.Replace("@", "@@");

			return "<style type=\"text/css\">" + css + "</style>";
		}
	}
}
