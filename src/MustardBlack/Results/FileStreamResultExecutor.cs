using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	sealed class FileStreamResultExecutor : ResultExecutor<FileStreamResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public FileStreamResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override async Task Execute(PipelineContext context, FileStreamResult result)
		{
			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;
			context.Response.ContentType = result.ContentType;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			if (!string.IsNullOrEmpty(result.ContentDisposition))
				context.Response.Headers.Add("Content-Disposition", result.ContentDisposition);

			using (result.FileStream)
			{
				var buffer = new byte[0x1000];
				while (true)
				{
					int bytesRead = await result.FileStream.ReadAsync(buffer, 0, 0x1000);
					if (bytesRead == 0)
						return;

					await context.Response.OutputStream.WriteAsync(buffer, 0, bytesRead);
				}
			}
		}
	}
}