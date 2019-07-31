using System.IO;
using System.Net;
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
		readonly IFileSystem fileSystem;

		public AreaStaticAssetHandler(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		public IResult Get(IRequest request)
		{
			if(request.Url.Path.ToLower() != request.Url.Path)
				return new PlainTextResult("Asset paths must be lowercase", HttpStatusCode.BadRequest);

			var path = '~' + request.Url.Path;

			if (this.fileSystem.Exists(path))
			{
				var fileStream = this.fileSystem.Open(path, FileMode.Open);
				var contentType = MimeMapping.GetMimeMapping(request.Url.Path);
				return new FileStreamResult(contentType, fileStream);
			}

			return ErrorResult.NotFound();
		}
	}
}
