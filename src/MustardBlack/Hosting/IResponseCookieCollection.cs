using System.Collections.Generic;

namespace MustardBlack.Hosting
{
	public interface IResponseCookieCollection : IEnumerable<ResponseCookie>
	{
		void Set(ResponseCookie item);

		/// <summary>
		/// Gett
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ResponseCookie Get(string name);
		void Clear();
		bool Remove(string name);
		int Count { get; }
	}
}
