using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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

		Task Write(string data);
		void SetCacheHeaders(IResult result);
	}
}