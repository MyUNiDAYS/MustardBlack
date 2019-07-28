using System.Collections.Specialized;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using MustardBlack.Results;

namespace MustardBlack.Hosting.AspNetCore
{
	public sealed class AspNetCoreResponse : IResponse
	{
		public AspNetCoreResponse(HttpContext context)
		{
			throw new System.NotImplementedException();
		}

		public NameValueCollection Headers { get; }
		public HttpStatusCode StatusCode { get; set; }
		public IResponseCookieCollection Cookies { get; }
		public string ContentType { get; set; }
		public Stream OutputStream { get; }
		public void Write(string body)
		{
			throw new System.NotImplementedException();
		}

		public void WriteFile(string path)
		{
			throw new System.NotImplementedException();
		}

		public void SetCacheHeaders(IResult result)
		{
			throw new System.NotImplementedException();
		}
	}
}