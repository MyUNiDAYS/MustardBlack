using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	sealed class FilePathResultExecutor : ResultExecutor<FilePathResult>
	{
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
			SetTempData(context, result.TempData);
		}
	}
}