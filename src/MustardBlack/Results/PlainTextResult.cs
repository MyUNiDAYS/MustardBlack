using System.Net;

namespace MustardBlack.Results
{
	public sealed class PlainTextResult : Result
	{
		public readonly string Data;

		public PlainTextResult(string data, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			this.Data = data;
			this.StatusCode = statusCode;
		}
	}
}