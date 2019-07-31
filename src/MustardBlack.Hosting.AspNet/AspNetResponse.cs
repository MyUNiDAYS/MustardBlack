using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using MustardBlack.Caching;
using MustardBlack.Results;
using HttpCacheRevalidation = MustardBlack.Caching.HttpCacheRevalidation;

namespace MustardBlack.Hosting.AspNet
{
	public sealed class AspNetResponse : IResponse
	{
		readonly HttpContext context;

		public NameValueCollection Headers => this.context.Response.Headers;

		public HttpStatusCode StatusCode
		{
			get => (HttpStatusCode) this.context.Response.StatusCode;
			set
			{
				this.context.Response.StatusCode = (int) value;
				this.context.Response.TrySkipIisCustomErrors = true;
			}
		}
		
		public void SetCacheHeaders(IResult result)
		{
			this.context.Response.Cache.SetLastModified(result.LastModified);

			// Turn this BS .NET thing off
			this.context.Response.Cache.SetSlidingExpiration(false);

			switch (result.CachePolicy)
			{
				case CachePolicy.NoStore:
					this.context.Response.Cache.SetNoStore();
					return;

				case CachePolicy.Private:
					this.context.Response.Cache.SetCacheability(HttpCacheability.Private);
					break;

				case CachePolicy.Public:
					this.context.Response.Cache.SetCacheability(HttpCacheability.Public);
					break;
			}

			if (result.Expires.HasValue)
				this.context.Response.Cache.SetExpires(result.Expires.Value);

			if (result.MaxAge.HasValue)
				this.context.Response.Cache.SetMaxAge(result.MaxAge.Value);

			var revalidation = GetCacheRevalidation(result.CacheRevalidation);
			this.context.Response.Cache.SetRevalidation(revalidation);
		}

		public IResponseCookieCollection Cookies { get; }

		public string ContentType
		{
			get => this.context.Response.ContentType;
			set => this.context.Response.ContentType = value;
		}

		public Stream OutputStream => this.context.Response.OutputStream;

		public Task Write(string data)
		{
			this.context.Response.Write(data);
			return Task.CompletedTask;
		}
		
		static System.Web.HttpCacheRevalidation GetCacheRevalidation(HttpCacheRevalidation revalidation)
		{
			switch (revalidation)
			{
				case HttpCacheRevalidation.AllCaches:
					return System.Web.HttpCacheRevalidation.AllCaches;
				case HttpCacheRevalidation.ProxyCaches:
					return System.Web.HttpCacheRevalidation.ProxyCaches;
				case HttpCacheRevalidation.None:
					return System.Web.HttpCacheRevalidation.None;
				default:
					throw new NotSupportedException();
			}
		}
		
		public AspNetResponse(HttpContext context)
		{
			this.context = context;
			// Stop it defaulting to text/html.
			this.context.Response.ContentType = null;
			this.Cookies = new AspNetResponseCookieCollection(context.Response.Cookies);
			this.context.Response.BufferOutput = true;
			this.StatusCode = HttpStatusCode.NotImplemented;
		}
	}
}