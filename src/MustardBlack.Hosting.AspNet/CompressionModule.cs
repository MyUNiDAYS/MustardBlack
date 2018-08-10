using System;
using System.Web;

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

			if (acceptedTypes.Contains("br"))
			{
				response.Filter = new BrotliStreamWrapper(response.Filter, () =>
				{
					response.AppendHeader("Vary", "Accept-Encoding");
					response.AppendHeader("Content-Encoding", "br");
				});
			}
			else if (acceptedTypes.Contains("gzip"))
			{
				response.Filter = new GzipStreamWrapper(response.Filter, () =>
				{
					response.AppendHeader("Vary", "Accept-Encoding");
					response.AppendHeader("Content-Encoding", "gzip");
				});
			}
			else if (acceptedTypes.Contains("deflate"))
			{
				response.Filter = new DeflateStreamWrapper(response.Filter, () =>
				{
					response.AppendHeader("Vary", "Accept-Encoding");
					response.AppendHeader("Content-Encoding", "deflate");
				});
			}
		}

		public void Dispose()
		{
		}
	}
}