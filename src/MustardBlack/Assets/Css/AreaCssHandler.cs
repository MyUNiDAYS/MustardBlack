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
		readonly ICssPreprocessor cssPreprocessor;

		public AreaCssHandler(IAssetLoader assetLoader, ICssPreprocessor cssPreprocessor)
		{
			this.assetLoader = assetLoader;
			this.cssPreprocessor = cssPreprocessor;
		}

		public IResult Get(IRequest request)
		{
			var area = request.Url.Path.Substring(1, request.Url.Path.IndexOf('.') - 1);
			var path = "~/areas/" + area + "/assets/styles/";

			var asset = this.assetLoader.GetAsset(path, this.cssPreprocessor.FileMatch);
			
			var assetResult = this.cssPreprocessor.Process(asset);
			if (assetResult.Status == AssetProcessingResult.CompilationStatus.Success)
				return new FileContentResult("text/css", Encoding.UTF8.GetBytes(assetResult.Result));

			return new FileContentResult("text/css", Encoding.UTF8.GetBytes(assetResult.Message), HttpStatusCode.InternalServerError);
		}
	}
}
