using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MustardBlack.Hosting.AspNetCore
{
	public sealed class AspNetCoreRequest : IRequest
	{
		readonly HttpContext context;

		public string IpAddress { get; }
		public uint IpLong { get; }
		public string UserAgent { get; }
		public ContentType ContentType { get; }
		public Url Url { get; }
		public HttpMethod HttpMethod { get; set; }
		public NameValueCollection Form { get; }
		public IDictionary<string, IEnumerable<IFile>> Files { get; }
		public HeaderCollection Headers { get; }
		public Stream BufferlessInputStream => this.context.Request.Body;
		public IRequestState State { get; }
		public IRequestCookieCollection Cookies { get; }
		public Url Referrer { get; }
		public NameValueCollection ServerVariables { get; }
		public IEnumerable<StringWithQualityHeaderValue> AcceptLanguages { get; }

		public AspNetCoreRequest(HttpContext context)
		{
			this.context = context;

			this.IpAddress = context.Connection.RemoteIpAddress.ToString();
			//this.IpLong = (uint)context.Connection.RemoteIpAddress.Address;

			this.UserAgent = context.Request.Headers["User-Agent"];

			this.ContentType = ParseContentType(context.Request.ContentType);


			this.Url = new Url(this.context.Request.Scheme, this.context.Request.Host.Host, this.context.Request.Host.Port ?? (this.context.Request.IsHttps ? 443 : 80), this.context.Request.Path + this.context.Request.QueryString);

			HttpMethod method;
			if (!Enum.TryParse(context.Request.Method, true, out method))
				throw new Exception("Cannot determine HttpMethod");
			this.HttpMethod = method;

			this.Form = new NameValueCollection();
			this.Files = new Dictionary<string, IEnumerable<IFile>>();

			if (this.HttpMethod == HttpMethod.Delete || this.HttpMethod == HttpMethod.Patch || this.HttpMethod == HttpMethod.Post || this.HttpMethod == HttpMethod.Put)
			{
				if (this.ContentType.MediaType == "application/x-www-form-urlencoded" || this.ContentType.MediaType == "multipart/form-data")
				{
					foreach (var key in this.context.Request.Form.Keys)
						this.Form.Add(key, this.context.Request.Form[key].ToString());

					foreach (var file in this.context.Request.Form.Files)
					{
						if (!this.Files.ContainsKey(file.Name))
							this.Files.Add(file.Name, new List<IFile>());

						var files = this.Files[file.Name] as List<IFile>;
						files.Add(new AspNetCoreFile(this.context.Request.Form.Files[file.Name]));
					}
				}
			}
			
			if (context.Request.Headers["Referer"] != StringValues.Empty)
			{
				try
				{
					this.Referrer = new Url(context.Request.Headers["Referer"].ToString());
				}
				catch
				{
				}
			}


			this.Headers = new HeaderCollection();
			foreach (var key in this.context.Request.Headers.Keys)
				this.Headers.Add(key, this.context.Request.Headers[key].ToString());

			this.ServerVariables = new HeaderCollection();
			foreach (var key in this.context.Request.Headers.Keys)
			{
				if(key.StartsWith("HTTP_"))
					this.ServerVariables.Add(key.Substring(5), this.context.Request.Headers[key]);
			}

			this.AcceptLanguages = ParseAcceptLanguages(context.Request.Headers["Accept-Language"]);

			this.State = new RequestState();

			var cookieHeader = this.context.Request.Headers["Cookie"];
			this.Cookies = new RequestCookieCollection(cookieHeader);
		}

		static ContentType ParseContentType(string requestContentType)
		{
			if (!string.IsNullOrEmpty(requestContentType))
			{
				try
				{
					return new ContentType(requestContentType.ToLowerInvariant());
				}
				catch
				{
					return new ContentType("application/octet-stream");
				}
			}

			return null;
		}
		static IEnumerable<StringWithQualityHeaderValue> ParseAcceptLanguages(string header)
		{
			if (!string.IsNullOrEmpty(header))
			{
				try
				{
					var languages = header.Split(',')
						.Select(StringWithQualityHeaderValue.Parse)
						.Select(a => new StringWithQualityHeaderValue(a.Value,
							a.Quality.GetValueOrDefault(1)))
						.OrderByDescending(a => a.Quality)
						.ToArray();

					return languages;
				}
				catch
				{
				}
			}

			return new StringWithQualityHeaderValue[0];
		}
	}
}