using System.Net;

namespace MustardBlack.Results
{
	public sealed class EmptyResult : Result
	{
		public EmptyResult(HttpStatusCode statusCode = HttpStatusCode.NoContent)
		{
			this.StatusCode = statusCode;
		}
	}
}
