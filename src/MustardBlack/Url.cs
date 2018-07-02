using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace MustardBlack
{
	/// <summary>
	/// Represents a URL, giving editable access to all fragments
	/// </summary>
	public class Url
	{
		static Uri baseUri = new Uri("https://base");

		public bool IsAbsolute { get; set; }
		public string Scheme { get; set; }

		/// <summary>
		/// Gets/sets the Host
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Gets/sets the port
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Gets/Sets the Path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Gets/Sets the QueryString fragment of the URL
		/// </summary>
		public string QueryString
		{
			get
			{
				var builder = new StringBuilder("?");

				int i = 0;
				foreach (var key in this.QueryCollection.AllKeys.OrderBy(x => x))
				{
					if (i++ != 0)
						builder.Append('&');

					if (!string.IsNullOrEmpty(key))
					{
						builder.Append(HttpUtility.UrlEncode(key));
						builder.Append('=');
					}

					if(this.QueryCollection[key] != null)
						builder.Append(HttpUtility.UrlEncode(this.QueryCollection[key]));
				}

				if (builder.Length == 1)
					return string.Empty;

				return builder.ToString();
			}
			set
			{
				var collection = HttpUtility.ParseQueryString(value);
				this.QueryCollection = new NameValueCollection();
				foreach (var key in collection.AllKeys)
					this.QueryCollection.Add(key, collection[key]);
			}
		}

		/// <summary>
		/// Returns the Path and Query fragments of the URL without the Data part
		/// </summary>
		public string PathAndQuery
		{
			get => string.Concat(this.Path, this.QueryString);
			set
			{
				var queryStart = value.IndexOf('?');
				if (queryStart == -1)
					queryStart = value.Length;

				var path = value.Substring(0, queryStart);
				this.Path = '/' + path.TrimLeading('/');

				this.QueryString = value.Substring(queryStart);
			}
		}

		/// <summary>
		/// Exposes an editable collection of key/value pairs representing the QueryString fragment's parts
		/// </summary>
		public NameValueCollection QueryCollection { get; protected set; }

		/// <summary>
		/// Exposes the hash fragment
		/// </summary>
		public string Fragment { get; set; }

		/// <summary>
		/// Creates a new URL class from the given string representation
		/// </summary>
		/// <param name="scheme"></param>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="relativeUrl"></param>
		/// <param name="querystringValues"></param>
		/// <param name="fragment"></param>
		public Url(string scheme, string host, int port, string relativeUrl, object querystringValues = null, string fragment = null)
		{
			this.Init(scheme, host, port, relativeUrl, querystringValues, fragment);
		}

		public Url(Url url)
		{
			this.IsAbsolute = url.IsAbsolute;
			this.Scheme = url.Scheme;
			this.Host = url.Host;
			this.Port = url.Port;
			this.Path = url.Path;
			this.Fragment = url.Fragment;
			this.QueryCollection = new NameValueCollection(url.QueryCollection);
		}

		public Url(string uri)
		{
			Uri u;
			var absolute = true;

			try
			{
				u = new Uri(uri, UriKind.RelativeOrAbsolute);
				if (!u.IsAbsoluteUri)
				{
					u = new Uri(baseUri, uri);
					absolute = false;
				}
			}
			catch (UriFormatException e)
			{
				throw new UriFormatException($"Error parsing `{uri}` as URI", e);
			}
			
			if(absolute)
				this.Init(u.Scheme, u.Host, u.Port, u.PathAndQuery, null, u.Fragment);
			else
				this.Init(u.PathAndQuery, null, u.Fragment);
		}

		public Url(Uri uri)
		{
			if (uri.IsAbsoluteUri)
			{
				this.Init(uri.Scheme, uri.Host, uri.Port, uri.PathAndQuery, null, uri.Fragment);
			}
			else
			{
				uri = new Uri(baseUri, uri);
				this.Init(uri.PathAndQuery, null, uri.Fragment);
			}
		}

		void Init(string scheme, string host, int port, string relativeUrl, object querystringValues, string fragment)
		{
			this.Init(relativeUrl, querystringValues, fragment);

			this.IsAbsolute = true;

			this.Scheme = scheme;
			this.Host = host;
			this.Port = port;
		}
		void Init(string relativeUrl, object querystringValues, string fragment)
		{
			this.IsAbsolute = false;

			this.Fragment = fragment;
			if (this.Fragment != null && this.Fragment.StartsWith("#"))
				this.Fragment = this.Fragment.Substring(1);
			
			var queryStart = relativeUrl.IndexOf('?');
			if (queryStart == -1)
				queryStart = relativeUrl.Length;

			var path = relativeUrl.Substring(0, queryStart);
			this.Path = '/' + path.TrimStart('/');

			this.QueryString = relativeUrl.Substring(queryStart);

			if (querystringValues != null)
			{
				var routeValueDictionary = querystringValues.ToDictionary();
				foreach (var key in routeValueDictionary.Keys)
					this.QueryCollection[key] = routeValueDictionary[key] != null ? routeValueDictionary[key].ToString() : null;
			}
		}

		/// <summary>
		/// Returns a string representation of the URL
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var builder = new StringBuilder();

			if (this.IsAbsolute)
			{
				builder.Append(this.Scheme)
					.Append("://")
					.Append(this.Host);

				if (this.Scheme.ToUpperInvariant() == "HTTPS" && this.Port != 443)
					builder.Append(':').Append(this.Port);

				if (this.Scheme.ToUpperInvariant() == "HTTP" && this.Port != 80)
					builder.Append(':').Append(this.Port);
			}

			builder.Append(this.Path).Append(this.QueryString);

			if (!string.IsNullOrWhiteSpace(this.Fragment))
				builder.Append('#').Append(this.Fragment);

			return builder.ToString();
		}


		/// <summary>
		/// Returns a relative (no scheme, host, port) URL from the given Url
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public Url ToRelativeUrl()
		{
			if (this.IsAbsolute)
				return new Url(this.PathAndQuery);

			return this;
		}

		public static implicit operator string(Url uri)
		{
			return uri?.ToString();
		}
		
		bool Equals(Url other)
		{
			return string.Equals(this.ToString(), other.ToString());
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return this.Equals((Url) obj);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}
	}
}
