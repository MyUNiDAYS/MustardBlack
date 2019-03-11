using System;
using System.Text;
using System.Text.RegularExpressions;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.Assets.Javascript
{
	/// <summary>
	/// Should always be mapped to handle "/{area}.js"
	/// </summary>
	public sealed class AreaJavascriptHandler : Handler
	{
		readonly IAssetLoader assetLoader;

		readonly IJavascriptPreprocessor javascriptPreprocessor;

		// Must end with ".js", but not ".test.js"
		public static readonly Regex FileMatch = new Regex(@".*(?<!test).js$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
		public AreaJavascriptHandler(IAssetLoader assetLoader, IJavascriptPreprocessor javascriptPreprocessor)
		{
			this.assetLoader = assetLoader;
			this.javascriptPreprocessor = javascriptPreprocessor;
		}

		public IResult Get(IRequest request)
		{
			var area = request.Url.Path.Substring(1, request.Url.Path.IndexOf('.') - 1);
			var path = "~/areas/" + area + "/assets/scripts/";

			var assets = this.assetLoader.GetAssets(path, FileMatch);

			var js = this.javascriptPreprocessor.Process(assets);

			return new FileContentResult("application/javascript", Encoding.UTF8.GetBytes(js));
		}
	}
}
