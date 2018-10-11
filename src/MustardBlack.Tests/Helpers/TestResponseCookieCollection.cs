using System;
using System.Collections;
using System.Collections.Generic;
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
