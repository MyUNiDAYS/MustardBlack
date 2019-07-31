using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MustardBlack.Hosting;
using MustardBlack.Results;
using Newtonsoft.Json;

namespace MustardBlack.Tests.Helpers
{
	public class TestResponse : IResponse
	{
		public NameValueCollection Headers { get; private set; }
		public HttpStatusCode StatusCode { get; set; }
		public IResponseCookieCollection Cookies { get; }
		public string ContentType { get; set; }
		public Stream OutputStream { get; private set; }

		public TestResponse()
		{
			this.Headers = new NameValueCollection();
			this.Cookies = new TestResponseCookieCollection();
			this.OutputStream = new MemoryStream();
		}

		public Task Write(string data)
		{
			var buffer = Encoding.UTF8.GetBytes(data);
			this.OutputStream.Write(buffer, 0, buffer.Length);
			return Task.CompletedTask;
		}

		public void WriteFile(string path)
		{
			throw new NotImplementedException();
		}

		public void SetCacheHeaders(IResult result)
		{
		}

		public void SuppressFormsAuthenticationRedirect()
		{
		}

		public string GetEntityString()
		{
			return Encoding.UTF8.GetString((this.OutputStream as MemoryStream).ToArray());
		}

		public dynamic GetEntityJson()
		{
			return JsonConvert.DeserializeObject(this.GetEntityString());
		}

		public IDictionary<string, IEnumerable<string>> GetJsonErrors()
		{
			return JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<string>>>(this.GetEntityString());
		}
	}
}
