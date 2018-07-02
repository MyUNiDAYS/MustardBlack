using MustardBlack.Pipeline;
using Newtonsoft.Json;

namespace MustardBlack.Results
{
	sealed class JsonResultExecutor : ResultExecutor<JsonResult>
	{
		public override void Execute(PipelineContext context, JsonResult result)
		{
			context.Response.ContentType = "application/json";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			SetTempData(context, result.TempData);

			if (result.Data != null)
			{
				var entity = JsonConvert.SerializeObject(result.Data, result.SerializerSettings);
				context.Response.Write(entity);
			}
		}
	}
}