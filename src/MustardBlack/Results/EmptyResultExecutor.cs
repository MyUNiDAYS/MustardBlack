using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	sealed class EmptyResultExecutor : ResultExecutor<EmptyResult>
	{
		public override void Execute(PipelineContext context, EmptyResult result)
		{
			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			SetTempData(context, result.TempData);
		}
	}
}