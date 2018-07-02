using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using MustardBlack.Hosting;

namespace MustardBlack.Tests.Helpers
{
	public class TestResponseCookieCollection : IResponseCookieCollection
	{
		readonly IDictionary<string, ResponseCookie> cookies;

		public TestResponseCookieCollection()
		{
			this.cookies = new Dictionary<string, ResponseCookie>(StringComparer.OrdinalIgnoreCase);
		}

		public IEnumerator<ResponseCookie> GetEnumerator()
		{
			return this.cookies.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Set(ResponseCookie cookie)
		{
			this.cookies.Add(cookie.Name, cookie);

			var httpCookie = new HttpCookie(cookie.Name, cookie.Value)
			{
				Domain = cookie.Domain,
				Path = cookie.Path,
				HttpOnly = cookie.HttpOnly,
				Secure = cookie.Secure
			};

			if (cookie.Expires.HasValue)
				httpCookie.Expires = cookie.Expires.Value;
		}

		public ResponseCookie Get(string name)
		{
			if (this.cookies.ContainsKey(name))
				return this.cookies[name];

			return null;
		}

		public void Clear()
		{
			this.cookies.Clear();
		}

		public bool Remove(string name)
		{
			var removed = this.cookies.ContainsKey(name);

			this.cookies.Remove(name);

			return removed;
		}

		public int Count => this.cookies.Values.Count;
	}
}
