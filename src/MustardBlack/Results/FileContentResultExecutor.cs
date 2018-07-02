using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	sealed class FileContentResultExecutor : ResultExecutor<FileContentResult>
	{
		public override void Execute(PipelineContext context, FileContentResult result)
		{
			context.Response.OutputStream.Write(result.Data, 0, result.Data.Length);

			if(!string.IsNullOrEmpty(result.ContentDisposition))
				context.Response.Headers.Add("Content-Disposition", result.ContentDisposition);

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;
			context.Response.ContentType = result.ContentType;

			SetLinkHeaders(context, result);
			SetTempData(context, result.TempData);
		}
	}
}