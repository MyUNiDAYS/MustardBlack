using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	sealed class PlainTextResultExecutor : ResultExecutor<PlainTextResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public PlainTextResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override async Task Execute(PipelineContext context, PlainTextResult result)
		{
			context.Response.ContentType = "text/plain";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			if (result.Data != null)
				await context.Response.Write(result.Data);
		}
	}
}