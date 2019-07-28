using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace MustardBlack.Hosting.AspNetCore
{
	public sealed class AspNetCoreRequest : IRequest
	{
		readonly HttpContext context;

		public AspNetCoreRequest(HttpContext context)
		{
			this.context = context;
			throw new System.NotImplementedException();
		}

		public string IpAddress { get; }
		public uint IpLong { get; }
		public string UserAgent { get; }
		public ContentType ContentType { get; }
		public Url Url { get; }
		public HttpMethod HttpMethod { get; set; }
		public NameValueCollection Form { get; }
		public IDictionary<string, IEnumerable<IFile>> Files { get; }
		public HeaderCollection Headers { get; }
		public Stream BufferlessInputStream { get; }
		public IRequestState State { get; }
		public IRequestCookieCollection Cookies { get; }
		public Url Referrer { get; }
		public NameValueCollection ServerVariables { get; }
		public IEnumerable<StringWithQualityHeaderValue> AcceptLanguages { get; }
	}
}