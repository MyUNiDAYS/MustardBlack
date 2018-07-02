using System.Net;
using Newtonsoft.Json;

namespace MustardBlack.Results
{
	public sealed class JsonResult : Result
	{
		public readonly object Data;
		public JsonSerializerSettings SerializerSettings { get; set; }

		public JsonResult(object data, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			this.Data = data;
			this.StatusCode = statusCode;
		}
	}
}
