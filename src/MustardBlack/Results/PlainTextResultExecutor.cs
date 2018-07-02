using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	sealed class PlainTextResultExecutor : ResultExecutor<PlainTextResult>
	{
		public override void Execute(PipelineContext context, PlainTextResult result)
		{
			context.Response.ContentType = "text/plain";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			SetTempData(context, result.TempData);

			if (result.Data != null)
				context.Response.Write(result.Data);
		}
	}
}