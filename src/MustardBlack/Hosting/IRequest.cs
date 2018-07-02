using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;

namespace MustardBlack.Hosting
{
	public interface IRequest
	{
		string IpAddress { get; }
		uint IpLong { get; }

		string UserAgent { get; }

		string ContentType { get; }

		/// <summary>
		/// The requested Url
		/// </summary>
		Url Url { get; }

		/// <summary>
		/// The HttpMethod of the request
		/// </summary>
		HttpMethod HttpMethod { get; set; }

		/// <summary>
		/// The request's FORM collection
		/// </summary>
		NameValueCollection Form { get; }

		IDictionary<string, IEnumerable<IFile>> Files { get; }

		HeaderCollection Headers { get; }
		
		/// <summary>
		/// Unbuffered input stream
		/// </summary>
		Stream BufferlessInputStream { get; }

		IRequestState State { get; }
		
		IRequestCookieCollection Cookies { get; }
		
		Url Referrer { get; }
		NameValueCollection ServerVariables { get; }
		IEnumerable<StringWithQualityHeaderValue> AcceptLanguages { get; }
	}
}
