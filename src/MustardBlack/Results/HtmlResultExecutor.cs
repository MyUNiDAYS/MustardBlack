using System.Text;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Results
{
	public sealed class HtmlResultExecutor : ResultExecutor<HtmlResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public HtmlResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override async Task Execute(PipelineContext context, HtmlResult result)
		{
			var data = Encoding.UTF8.GetBytes(result.Html);
			await context.Response.OutputStream.WriteAsync(data, 0, data.Length);

			context.Response.ContentType = "text/html";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);
		}
	}
}