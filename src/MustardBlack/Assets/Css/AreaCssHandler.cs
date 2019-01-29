using System.Collections.Generic;
using System.Net;
using System.Text;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.Assets.Css
{
	/// <summary>
	/// Should always be mapped to handle "/{area}.css"
	/// </summary>
	public sealed class AreaCssHandler : Handler
	{
		readonly IAssetLoader assetLoader;
		readonly IEnumerable<ICssPreprocessor> cssPreprocessors;

		public AreaCssHandler(IAssetLoader assetLoader, IEnumerable<ICssPreprocessor> cssPreprocessors)
		{
			this.assetLoader = assetLoader;
			this.cssPreprocessors = cssPreprocessors;
		}

		public IResult Get(IRequest request)
		{
			var area = request.Url.Path.Substring(1, request.Url.Path.IndexOf('.') - 1);
			var path = "~/areas/" + area + "/assets/styles/";

			foreach (var cssPreprocessor in this.cssPreprocessors)
			{
				var asset = this.assetLoader.GetAsset(path, cssPreprocessor.FileMatch);

				if (string.IsNullOrWhiteSpace(asset))
					continue;

				var assetResult = cssPreprocessor.Process(asset);
				if (assetResult.Status == AssetProcessingResult.CompilationStatus.Success)
					return new FileContentResult("text/css", Encoding.UTF8.GetBytes(assetResult.Result));
			}

			return new EmptyResult(HttpStatusCode.NotFound);
		}
	}
}
