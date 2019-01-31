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

		public override Task Execute(PipelineContext context, HtmlResult result)
		{
			var data = Encoding.UTF8.GetBytes(result.Html);
			context.Response.OutputStream.Write(data, 0, data.Length);

			context.Response.ContentType = "text/html";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			return Task.CompletedTask;
		}
	}
}