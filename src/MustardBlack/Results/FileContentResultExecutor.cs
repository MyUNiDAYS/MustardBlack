using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	sealed class FileContentResultExecutor : ResultExecutor<FileContentResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public FileContentResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override Task Execute(PipelineContext context, FileContentResult result)
		{
			context.Response.OutputStream.Write(result.Data, 0, result.Data.Length);

			if(!string.IsNullOrEmpty(result.ContentDisposition))
				context.Response.Headers.Add("Content-Disposition", result.ContentDisposition);

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;
			context.Response.ContentType = result.ContentType;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			return Task.CompletedTask;
		}
	}
}