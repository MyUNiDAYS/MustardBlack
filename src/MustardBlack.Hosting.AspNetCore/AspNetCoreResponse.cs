using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MustardBlack.Results;

namespace MustardBlack.Hosting.AspNetCore
{
	public sealed class AspNetCoreResponse : IResponse
	{
		readonly HttpContext context;
		public NameValueCollection Headers { get; }
		public HttpStatusCode StatusCode
		{
			get => (HttpStatusCode)this.context.Response.StatusCode;
			set => this.context.Response.StatusCode = (int)value;
		}

		public IResponseCookieCollection Cookies { get; }
		public string ContentType
		{
			get => this.context.Response.ContentType;
			set => this.context.Response.ContentType = value;
		}

		public Stream OutputStream => this.context.Response.Body;

		public AspNetCoreResponse(HttpContext context)
		{
			this.context = context;
			this.Cookies = new AspNetCoreResponseCookieCollection(context.Response.Cookies);
			this.Headers = new HeaderCollection();
		}

		public Task Write(string data)
		{
			return this.context.Response.WriteAsync(data);
		}
		
		public void SetCacheHeaders(IResult result)
		{
			
		}
	}
}