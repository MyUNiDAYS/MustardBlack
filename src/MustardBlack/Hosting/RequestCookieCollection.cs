using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MustardBlack.Hosting
{
	public sealed class RequestCookieCollection : IRequestCookieCollection
	{
		readonly Dictionary<string, RequestCookie> cookies;

		public RequestCookieCollection(string cookieHeader = null)
		{
			this.cookies = new Dictionary<string, RequestCookie>(StringComparer.OrdinalIgnoreCase);
			ParseCookies(cookieHeader);
		}

		public RequestCookie Get(string name)
		{
			if (this.cookies.ContainsKey(name))
				return this.cookies[name];

			return null;
		}

		public void Set(RequestCookie cookie)
		{
			this.cookies[cookie.Name] = cookie;
		}

		public void Clear()
		{
			this.cookies.Clear();
		}
		void ParseCookies(string cookieHeader)
		{
			if (string.IsNullOrEmpty(cookieHeader))
				return;

			var values = cookieHeader.TrimEnd(';', ',').Split(';', ',').Select(v => v.Trim());

			foreach (var parts in values.Select(c => c.Split(new[] { '=' }, 2)))
			{
				var cookieName = parts[0].Trim();

				string cookieValue;

				if (parts.Length == 1)
				{
					//Cookie attribute
					cookieValue = string.Empty;
				}
				else
				{
					cookieValue = parts[1];
				}

				this.cookies[cookieName] = new RequestCookie(cookieName, cookieValue);
			}
		}

		public IEnumerator<RequestCookie> GetEnumerator()
		{
			return this.cookies.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}