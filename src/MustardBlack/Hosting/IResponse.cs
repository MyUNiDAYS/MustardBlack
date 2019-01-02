using System.Collections.Specialized;
using System.IO;
using System.Net;
using MustardBlack.Results;

namespace MustardBlack.Hosting
{
	public interface IResponse
	{
		NameValueCollection Headers { get; }
		HttpStatusCode StatusCode { get; set; }
		
		IResponseCookieCollection Cookies { get; }
		string ContentType { get; set; }
		Stream OutputStream { get; }

		void Write(string body);
		void WriteFile(string path);
		void SetCacheHeaders(IResult result);
	}
}