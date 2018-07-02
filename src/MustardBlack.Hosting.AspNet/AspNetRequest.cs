using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using MustardBlack.Hosting;
using UD.Core.Extensions;

namespace MustardBlack.AspNet
{
	public sealed class AspNetRequest : IRequest
	{
		readonly HttpContextBase context;
		public string ContentType { get; }
		public Url Url { get; }
		public HttpMethod HttpMethod { get; set; }

		// Like this to support reading of Request.GetBufferlessInputStream
		NameValueCollection form;
		public NameValueCollection Form => this.form ?? (this.form = this.context.Request.Form);

		// Like this to support reading of Request.GetBufferlessInputStream
		IDictionary<string, IEnumerable<IFile>> files;
		public IDictionary<string, IEnumerable<IFile>> Files => this.files ?? (this.files = GetFiles(this.context.Request.Files));

		public HeaderCollection Headers { get; }
		public IEnumerable<StringWithQualityHeaderValue> AcceptLanguages { get; private set; }
		public NameValueCollection ServerVariables { get; }

		public Stream BufferlessInputStream => this.context.Request.GetBufferlessInputStream();

        public IRequestState State { get; }
		public IRequestCookieCollection Cookies { get; }

		public Url Referrer { get; }
		public string IpAddress { get; }
		public uint IpLong { get; }
		public string UserAgent { get; }

		public AspNetRequest(HttpContext context) : this(new HttpContextWrapper(context))
		{
		}

		public AspNetRequest(HttpContextBase context)
		{
			this.context = context;

			this.IpAddress = context.Request.UserIpAddress();
			this.IpLong = context.Request.UserIp();

			this.UserAgent = context.Request.UserAgent;

			this.ContentType = ParseContentType(context.Request.ContentType);

			this.ServerVariables = context.Request.ServerVariables;

			var currentUri = context.Request.Url;

			// Workaround ELB TLS termination
			var port = context.Request.Url.Port;
			if (port == 80 && (context.Request.Headers["X-Forwarded-Proto"]?.Equals("https", StringComparison.InvariantCultureIgnoreCase) ?? false))
				port = 443;
			
			this.Url = new Url(currentUri.Scheme, currentUri.Host, port, currentUri.PathAndQuery);

			if (context.Request.UrlReferrer != null)
			{
				try
				{
					this.Referrer = new Url(context.Request.UrlReferrer);
				}
				catch
				{
				}
			}
			
			HttpMethod method;
			if (!Enum.TryParse(context.Request.HttpMethod, true, out method))
				throw new HttpParseException("Cannot determine HttpMethod");
			this.HttpMethod = method;

			this.Headers = new HeaderCollection(context.Request.Headers);

			this.AcceptLanguages = ParseAcceptLanguages(context.Request.Headers["Accept-Language"]);
			
			this.State = new RequestState();

			var cookieHeader = this.context.Request.Headers["Cookie"];
			this.Cookies = new RequestCookieCollection(cookieHeader);
		}

		
		static string ParseContentType(string requestContentType)
		{
			// example "application/json ; charset=utf8" -> "application/json"
			return requestContentType?.Split(';').First().Trim().ToLowerInvariant() ?? string.Empty;
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
		
		static HttpPostedFileBase GetFile(HttpPostedFileBase file)
		{
			if (file == null)
				return null;

			if (file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
				return null;

			return file;
		}

		static IDictionary<string, IEnumerable<IFile>> GetFiles(HttpFileCollectionBase files)
		{
			var source = new List<KeyValuePair<string, IFile>>();

			for (var i = 0; i < files.Count; i++)
			{
				var key = files.AllKeys[i];
				if (key == null)
					continue;

				var file = GetFile(files[i]);
				if (file != null)
					source.Add(new KeyValuePair<string, IFile>(key, new AspNetFile(file)));
			}

			var groupedFiles = source.GroupBy(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

			var dictionary = groupedFiles.ToDictionary(x => x.Key, x => x.ToArray().AsEnumerable());

			return dictionary;
		}
	}
}
