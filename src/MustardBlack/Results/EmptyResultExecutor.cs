using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	sealed class EmptyResultExecutor : ResultExecutor<EmptyResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public EmptyResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override void Execute(PipelineContext context, EmptyResult result)
		{
			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);

			this.tempDataMechanism.SetTempData(context, result.TempData);
		}
	}
}