using System.Net;
using System.Web;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.Assets.Static
{
	/// <summary>
	/// Handles mappings for all static assets
	/// </summary>
	public sealed class AreaStaticAssetHandler : Handler
	{
		public IResult Get(IRequest request)
		{
			if(request.Url.Path.ToLower() != request.Url.Path)
				return new PlainTextResult("Asset paths must be lowercase", HttpStatusCode.BadRequest);

			var contentType = MimeMapping.GetMimeMapping(request.Url.Path);
			return new FilePathResult(contentType, '~' + request.Url.Path);
		}
	}
}
