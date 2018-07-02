using System.Collections.Generic;

namespace MustardBlack.Hosting
{
	public interface IRequestCookieCollection : IEnumerable<RequestCookie>
	{
		/// <summary>
		/// Gets a cookie by name - case insensitive
		/// </summary>
		/// <param name="name"></param>
		/// <returns>Null or a RequestCookie</returns>
		RequestCookie Get(string name);
		void Set(RequestCookie cookie);
		void Clear();
	}
}
