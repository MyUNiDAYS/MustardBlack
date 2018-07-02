using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	sealed class RedirectResultExecutor : ResultExecutor<RedirectResult>
	{
		public override void Execute(PipelineContext context, RedirectResult result)
		{
			context.Response.StatusCode = result.StatusCode;
			context.Response.SetCacheHeaders(result);
			context.Response.Headers.Set("Location", result.Location);

			SetLinkHeaders(context, result);
			SetTempData(context, result.TempData);
		}
	}
}