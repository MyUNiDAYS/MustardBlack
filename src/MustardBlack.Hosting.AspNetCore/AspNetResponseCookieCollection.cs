using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MustardBlack.Hosting.AspNetCore
{
	sealed class AspNetCoreResponseCookieCollection : IResponseCookieCollection
	{
		readonly IResponseCookies responseCookies;
		readonly IDictionary<string, ResponseCookie> cookies;
		public int Count => this.cookies.Values.Count;


		public AspNetCoreResponseCookieCollection(IResponseCookies responseCookies)
		{
			this.responseCookies = responseCookies;
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
			
			this.responseCookies.Append(cookie.Name, cookie.Value, new CookieOptions
			{
				Domain = cookie.Domain,
				Expires = cookie.Expires,
				HttpOnly = cookie.HttpOnly,
				Path = cookie.Path,
				Secure = cookie.Secure
			});
		}

		public ResponseCookie Get(string name)
		{
			if (this.cookies.ContainsKey(name))
				return this.cookies[name];

			return null;
		}

		public void Clear()
		{
			foreach(var key in this.cookies.Keys)
				this.responseCookies.Delete(key);

			this.cookies.Clear();
		}

		public bool Remove(string name)
		{
			var removed = this.cookies.ContainsKey(name);

			this.cookies.Remove(name);
			this.responseCookies.Delete(name);

			return removed;
		}
	}
}