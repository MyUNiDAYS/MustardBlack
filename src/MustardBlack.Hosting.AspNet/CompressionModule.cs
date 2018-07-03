using System;
using System.Web;
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

			response.AppendHeader("Vary", "Accept-Encoding");

			var acceptedTypes = request.Headers["Accept-Encoding"];
			if (acceptedTypes == null)
				return;
			
			if (acceptedTypes.Contains("gzip"))
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
