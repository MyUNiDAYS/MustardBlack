using System.Text;
using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	public sealed class HtmlResultExecutor : ResultExecutor<HtmlResult>
	{
		public override void Execute(PipelineContext context, HtmlResult result)
		{
			var data = Encoding.UTF8.GetBytes(result.Html);
			context.Response.OutputStream.Write(data, 0, data.Length);

			context.Response.ContentType = "text/html";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			SetTempData(context, result.TempData);
		}
	}
}