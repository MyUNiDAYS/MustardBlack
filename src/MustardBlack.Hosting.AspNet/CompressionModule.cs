using System;
using System.Web;
using Brotli;
using Ionic.Zlib;

namespace MustardBlack.Hosting.AspNet
{
	public sealed class CompressionModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.ReleaseRequestState += SetResponseCompressionFilter;
		}

		static void SetResponseCompressionFilter(object sender, EventArgs e)
		{
			var application = sender as HttpApplication;

			if (application == null)
				return;

			if (application.Context.Items.Contains("AlreadyCompressed"))
				return;

			application.Context.Items.Add("AlreadyCompressed", true);

			ApplyCompressionFilter(application.Request, application.Response);
		}

		static void ApplyCompressionFilter(HttpRequest request, HttpResponse response)
		{
			if (response.Filter == null)
				return;

			var acceptedTypes = request.Headers["Accept-Encoding"];
			if (acceptedTypes == null)
				return;

			if (response.Filter.Length == 0)
				return;

			response.AppendHeader("Vary", "Accept-Encoding");

			if (acceptedTypes.Contains("br"))
			{
				response.Filter = new BrotliStream(response.Filter, System.IO.Compression.CompressionMode.Compress, false);
				response.AppendHeader("Content-Encoding", "br");
			}
			else if (acceptedTypes.Contains("gzip"))
			{
				response.Filter = new GZipStream(response.Filter, CompressionMode.Compress, CompressionLevel.BestSpeed, false);
				response.AppendHeader("Content-Encoding", "gzip");
			}
			else if (acceptedTypes.Contains("deflate"))
			{
				response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress, CompressionLevel.BestSpeed, false);
				response.AppendHeader("Content-Encoding", "deflate");
			}
		}

		public void Dispose()
		{
		}
	}
}