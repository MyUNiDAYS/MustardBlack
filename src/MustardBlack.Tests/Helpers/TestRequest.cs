using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using MustardBlack.Hosting;

namespace MustardBlack.Tests.Helpers
{
	public class TestRequest : IRequest
	{
		public string IpAddress { get; set; }
		public uint IpLong { get; set; }
		public string UserAgent { get; set; }
		public ContentType ContentType { get; set; }
		public Url Url { get; set; }
		public HttpMethod HttpMethod { get; set; }
		public NameValueCollection Form { get; set; }
		public IDictionary<string, IEnumerable<IFile>> Files { get; set; }
		public HeaderCollection Headers { get; set; }
		public Stream BufferlessInputStream { get; set; }
		public IRequestState State { get; set; }
		public IRequestCookieCollection Cookies { get; }
		public Url Referrer { get; private set; }
		public NameValueCollection ServerVariables { get; private set; }
		public IEnumerable<StringWithQualityHeaderValue> AcceptLanguages { get; }

		public TestRequest()
		{
			this.Form = new NameValueCollection();
			this.Cookies = new RequestCookieCollection();
			this.Url = new Url("https://www.unidays.test/");
			this.State = new RequestState();
			this.Headers = new HeaderCollection();
			this.HttpMethod = HttpMethod.Get;
			this.Referrer = new Url("https://referrer.unidays.test");
			this.IpLong = 1337;
			this.ServerVariables = new NameValueCollection();
			this.BufferlessInputStream = new MemoryStream();
			this.AcceptLanguages = new StringWithQualityHeaderValue[0];
		}

		public void SetFormValues(object obj)
		{
			this.ContentType = "application/x-www-form-urlencoded";

			var dictionary = obj.ToDictionary();
			foreach(var pair in dictionary)
				this.Form.Add(pair.Key, pair.Value.ToString());
		}

		public void SetRequestBody(string body)
		{
			var data = Encoding.UTF8.GetBytes(body);
			this.BufferlessInputStream.Write(data, 0, data.Length);
			this.BufferlessInputStream.Position = 0;
		}

		public void SetFeatureFlagEnabled(string featureFlag)
		{
			this.Cookies.Set(new RequestCookie("ud-features", featureFlag));
		}

		public void SetFeatureFlagsEnabled(string[] featureFlags)
		{
			this.Cookies.Set(new RequestCookie("ud-features", string.Join("&", featureFlags)));
		}
	}
}
