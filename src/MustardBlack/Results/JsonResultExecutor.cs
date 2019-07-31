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

		public override Task Execute(PipelineContext context, JsonResult result)
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
					return context.Response.Write(json);
				}

				var entity = JsonConvert.SerializeObject(result.Data, result.SerializerSettings);
				return context.Response.Write(entity);
			}

			return Task.CompletedTask;
		}
	}
}