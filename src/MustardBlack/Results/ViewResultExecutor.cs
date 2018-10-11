using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using MustardBlack.Pipeline;
using MustardBlack.ViewEngines;
using Serilog;

namespace MustardBlack.Results
{
	sealed class ViewResultExecutor : ResultExecutor<ViewResult>
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
		
		readonly IViewRendererFinder viewRendererFinder;

		public ViewResultExecutor(IViewRendererFinder viewRendererFinder)
		{
			this.viewRendererFinder = viewRendererFinder;
		}

		public override void Execute(PipelineContext context, ViewResult result)
		{
			var rendered = new StringBuilder();
			
			var renderer = this.viewRendererFinder.FindViewRenderer(result.ViewType);

			var renderingContext = new ViewRenderingContext
			{
				RequestUrl = context.Request.Url,
				RequestState = context.Request.State,
				ContextItems = context.Items,
				Writer = new StringWriter(rendered)
			};

			try
			{
				renderer.Render(result, renderingContext).GetAwaiter().GetResult();
			}
			catch (Exception e)
			{
				log.Error(e, "Error rendering view `{ViewName}`", result.ViewType.FullName);
#if DEBUG
				if (Debugger.IsAttached) Debugger.Break();
#endif

				// TODO: render something useful
				context.Response.ContentType = "text/plain";
				context.Response.StatusCode = HttpStatusCode.InternalServerError;
				return;
			}

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;
			context.Response.Write(rendered.ToString());
			context.Response.ContentType = "text/html";

			SetLinkHeaders(context, result);
			SetTempData(context, result.TempData);
		}
	}
}