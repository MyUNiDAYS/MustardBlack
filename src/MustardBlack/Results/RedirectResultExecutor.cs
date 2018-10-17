using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	sealed class RedirectResultExecutor : ResultExecutor<RedirectResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public RedirectResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override void Execute(PipelineContext context, RedirectResult result)
		{
			context.Response.StatusCode = result.StatusCode;
			context.Response.SetCacheHeaders(result);
			context.Response.Headers.Set("Location", result.Location);

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);
		}
	}
}