using System;
using MustardBlack;
using MustardBlack.Hosting;

namespace UD.Core.Web.Hosting
{
	/// <summary>
	/// Extension methods for the IRequest interface
	/// </summary>
	public static class RequestExtensions
	{
		/// <summary>
		/// Determines if the request was made via AJAX
		/// </summary>
		/// <param name="request"></param>
		/// <returns>True if the request was made via AJAX.</returns>
		public static bool IsAjaxRequest(this IRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if(request.Headers["X-Requested-With"] == "XMLHttpRequest" || request.Headers["X-Requested-With"] == "fetch")
				return true;

			// CORS is ajax, but doesnt look like it
			var originHeader = request.Headers.Origin();
			if (originHeader != null)
			{
				var originUrl = new Url(originHeader);
				if (originUrl.Host != request.Url.Host && originUrl.Host.EndsWith(request.Url.Domain()))
					return true;
			}
			
			return false;
		}

		public static RequestType RequestType(this IRequest request)
		{
			return request.IsAjaxRequest() ? MustardBlack.RequestType.Ajax : MustardBlack.RequestType.Direct;
		}
    }
}