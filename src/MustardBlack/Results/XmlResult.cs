using System.Net;

namespace MustardBlack.Results
{
	public class XmlResult : Result
	{
		public object Data { get; }

		public XmlResult(object data, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			this.Data = data;
			this.StatusCode = statusCode;
		}
	}
}