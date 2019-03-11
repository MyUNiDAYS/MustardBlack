using System.Collections.Generic;
using System.Linq;
using MustardBlack.Assets;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using MustardBlack.Hosting;

namespace MustardBlack.ViewEngines.Razor
{
	public class AssetEnrichedRazorViewCompiler : RazorViewCompiler
	{
		readonly IJavascriptPreprocessor jsPreprocessor;
		readonly IEnumerable<ICssPreprocessor> cssPreprocessors;
		readonly IAssetLoader assetLoader;

		public AssetEnrichedRazorViewCompiler(IJavascriptPreprocessor jsPreprocessor, IEnumerable<ICssPreprocessor> cssPreprocessors, IFileSystem fileSystem, IAssetLoader assetLoader, IRazorConfiguration razorConfiguration) : base(fileSystem, razorConfiguration)
		{
			this.jsPreprocessor = jsPreprocessor;
			this.cssPreprocessors = cssPreprocessors;
			this.assetLoader = assetLoader;
		}
		
		public string PrepareJsForRazorCompilation(IEnumerable<AssetContent> assets)
		{
			if (!assets.Any())
				return null;
			
			var javascript = this.jsPreprocessor.Process(assets);

			javascript = javascript.Replace("@", "@@");
			
			// There are certain ways "<" inside <text></text> can confuse the Razor parser.
			// This seems to only be an issue when not compressed, although that is pure chance
			// Example parser confusing syntax: `@{<text>if(1 < 2) f('=');</text>}`
			// Hopefully this works around them all
			javascript = javascript.Replace("<", "@(_lessThanSymbol)");
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
