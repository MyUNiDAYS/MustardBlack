using System;
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
		readonly IAssetCache assetCache;

		public AreaCssHandler(IAssetLoader assetLoader, IEnumerable<ICssPreprocessor> cssPreprocessors, IAssetCache assetCache)
		{
			this.assetLoader = assetLoader;
			this.cssPreprocessors = cssPreprocessors;
			this.assetCache = assetCache;
		}

		public IResult Get(IRequest request)
		{
			var area = request.Url.Path.Substring(1, request.Url.Path.IndexOf('.') - 1);
			var path = "~/areas/" + area + "/assets/styles/";

			foreach (var cssPreprocessor in this.cssPreprocessors)
			{
				var lastModified = this.assetLoader.GetMaxLastModified(path, cssPreprocessor.FileMatch);

				// Didn't find any files
				if (lastModified == DateTime.MinValue)
					continue;

				var assetResult = this.assetCache.GetAsset(path, () => lastModified, () =>
				{
					var rawAsset = this.assetLoader.GetAsset(path, cssPreprocessor.FileMatch);
					
					var result = cssPreprocessor.Process(rawAsset);
					return result;
				});

				if (assetResult.Status == AssetProcessingResult.CompilationStatus.Success)
					return new FileContentResult("text/css", Encoding.UTF8.GetBytes(assetResult.Result));
				if (assetResult.Status == AssetProcessingResult.CompilationStatus.Failure)
					return new FileContentResult("text/plain", Encoding.UTF8.GetBytes(assetResult.Message), HttpStatusCode.InternalServerError);
			}

			return new EmptyResult(HttpStatusCode.NotFound);
		}
	}
}
