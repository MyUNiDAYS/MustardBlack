using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	sealed class FilePathResultExecutor : ResultExecutor<FilePathResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public FilePathResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override void Execute(PipelineContext context, FilePathResult result)
		{
			if (!string.IsNullOrEmpty(result.ContentDisposition))
				context.Response.Headers.Add("Content-Disposition", result.ContentDisposition);

			if (!string.IsNullOrEmpty(result.ContentType))
				context.Response.ContentType = result.ContentType;

			context.Response.WriteFile(result.Path);
			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);
		}
	}
}