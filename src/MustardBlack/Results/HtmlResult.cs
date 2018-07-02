using System.Net;

namespace MustardBlack.Results
{
	public sealed class HtmlResult : Result
	{
		public readonly string Html;

		public HtmlResult(string html, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			this.Html = html;
			this.StatusCode = statusCode;
		}
	}
}