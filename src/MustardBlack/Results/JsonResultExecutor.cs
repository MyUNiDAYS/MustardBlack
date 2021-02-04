using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.TempData;
using Newtonsoft.Json;

namespace MustardBlack.Results
{
	sealed class JsonResultExecutor : ResultExecutor<JsonResult>
	{
		readonly ITempDataMechanism tempDataMechanism;

		public JsonResultExecutor(ITempDataMechanism tempDataMechanism)
		{
			this.tempDataMechanism = tempDataMechanism;
		}

		public override async Task Execute(PipelineContext context, JsonResult result)
		{
			context.Response.ContentType = "application/json";

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			if (result.Data != null)
			{
				if (result.Data is string json)
				{
					await context.Response.Write(json);
				}
				else
				{
					var entity = JsonConvert.SerializeObject(result.Data, result.SerializerSettings);
					await context.Response.Write(entity);
				}
			}
		}
	}
}