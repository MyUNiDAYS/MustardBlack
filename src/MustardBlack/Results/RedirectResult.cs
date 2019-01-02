using System.Net;

namespace MustardBlack.Results
{
	public class RedirectResult : Result
	{
		public readonly Url Location;

		public RedirectResult(HttpStatusCode statusCode, Url location)
		{
			this.StatusCode = statusCode;
			this.Location = location;
		}

		/// <summary>
		/// The requested resource has been assigned a new permanent URI and any future references to this resource SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references to the Request-URI to one or more of the new references returned by the server, where possible. This response is cacheable unless indicated otherwise.
		/// The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).
		/// If the 301 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued.
		/// Note: When automatically redirecting a POST request after receiving a 301 status code, some existing HTTP/1.0 user agents will erroneously change it into a GET request.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult MovedPermanently(string location)
		{
			return new RedirectResult(HttpStatusCode.MovedPermanently, new Url(location));
		}

		/// <summary>
		/// The requested resource has been assigned a new permanent URI and any future references to this resource SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references to the Request-URI to one or more of the new references returned by the server, where possible. This response is cacheable unless indicated otherwise.
		/// The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).
		/// If the 301 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued.
		/// Note: When automatically redirecting a POST request after receiving a 301 status code, some existing HTTP/1.0 user agents will erroneously change it into a GET request.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult MovedPermanently(Url location)
		{
			return new RedirectResult(HttpStatusCode.MovedPermanently, location);
		}

		/// <summary>
		/// The requested resource resides temporarily under a different URI. Since the redirection might be altered on occasion, the client SHOULD continue to use the Request-URI for future requests. This response is only cacheable if indicated by a Cache-Control or Expires header field.
		/// The temporary URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).
		/// If the 302 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued.
		/// Note: RFC 1945 and RFC 2068 specify that the client is not allowed to change the method on the redirected request.  However, most existing user agent implementations treat 302 as if it were a 303 response, performing a GET on the Location field-value regardless of the original request method. The status codes 303 and 307 have been added for servers that wish to make unambiguously clear which kind of reaction is expected of the client.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult Found(string location)
		{
			return new RedirectResult(HttpStatusCode.Found, new Url(location));
		}
		/// <summary>
		/// The requested resource resides temporarily under a different URI. Since the redirection might be altered on occasion, the client SHOULD continue to use the Request-URI for future requests. This response is only cacheable if indicated by a Cache-Control or Expires header field.
		/// The temporary URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).
		/// If the 302 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued.
		/// Note: RFC 1945 and RFC 2068 specify that the client is not allowed to change the method on the redirected request.  However, most existing user agent implementations treat 302 as if it were a 303 response, performing a GET on the Location field-value regardless of the original request method. The status codes 303 and 307 have been added for servers that wish to make unambiguously clear which kind of reaction is expected of the client.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult Found(Url location)
		{
			return new RedirectResult(HttpStatusCode.Found, location);
		}

		/// <summary>
		/// The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource. This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource. The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the response to the second (redirected) request might be cacheable.
		/// The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).
		/// Note: Many pre-HTTP/1.1 user agents do not understand the 303 status. When interoperability with such clients is a concern, the 302 status code may be used instead, since most user agents react to a 302 response as described here for 303.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult SeeOther(string location)
		{
			return new RedirectResult(HttpStatusCode.SeeOther, new Url(location));
		}

		/// <summary>
		/// The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource. This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource. The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the response to the second (redirected) request might be cacheable.
		/// The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s).
		/// Note: Many pre-HTTP/1.1 user agents do not understand the 303 status. When interoperability with such clients is a concern, the 302 status code may be used instead, since most user agents react to a 302 response as described here for 303.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult SeeOther(Url location)
		{
			return new RedirectResult(HttpStatusCode.SeeOther, location);
		}

		/// <summary>
		/// The requested resource MUST be accessed through the proxy given by the Location field. The Location field gives the URI of the proxy. The recipient is expected to repeat this single request via the proxy. 305 responses MUST only be generated by origin servers.
		/// Note: RFC 2068 was not clear that 305 was intended to redirect a single request, and to be generated by origin servers only. Not observing these limitations has significant security consequences.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult UseProxy(string location)
		{
			return new RedirectResult(HttpStatusCode.UseProxy, new Url(location));
		}

		/// <summary>
		/// The requested resource MUST be accessed through the proxy given by the Location field. The Location field gives the URI of the proxy. The recipient is expected to repeat this single request via the proxy. 305 responses MUST only be generated by origin servers.
		/// Note: RFC 2068 was not clear that 305 was intended to redirect a single request, and to be generated by origin servers only. Not observing these limitations has significant security consequences.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult UseProxy(Url location)
		{
			return new RedirectResult(HttpStatusCode.UseProxy, location);
		}

		/// <summary>
		/// The requested resource resides temporarily under a different URI. Since the redirection MAY be altered on occasion, the client SHOULD continue to use the Request-URI for future requests. This response is only cacheable if indicated by a Cache-Control or Expires header field.
		/// The temporary URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s) , since many pre-HTTP/1.1 user agents do not understand the 307 status. Therefore, the note SHOULD contain the information necessary for a user to repeat the original request on the new URI.
		/// If the 307 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult Temporary(string location)
		{
			return new RedirectResult(HttpStatusCode.TemporaryRedirect, new Url(location));
		}

		/// <summary>
		/// The requested resource resides temporarily under a different URI. Since the redirection MAY be altered on occasion, the client SHOULD continue to use the Request-URI for future requests. This response is only cacheable if indicated by a Cache-Control or Expires header field.
		/// The temporary URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s) , since many pre-HTTP/1.1 user agents do not understand the 307 status. Therefore, the note SHOULD contain the information necessary for a user to repeat the original request on the new URI.
		/// If the 307 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued.
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public static RedirectResult Temporary(Url location)
		{
			return new RedirectResult(HttpStatusCode.TemporaryRedirect, location);
		}
	}
}
