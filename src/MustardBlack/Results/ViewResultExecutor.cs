using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.TempData;
using MustardBlack.ViewEngines;
using Serilog;

namespace MustardBlack.Results
{
	sealed class ViewResultExecutor : ResultExecutor<ViewResult>
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
		
		readonly IViewRendererFinder viewRendererFinder;
		readonly ITempDataMechanism tempDataMechanism;

		public ViewResultExecutor(IViewRendererFinder viewRendererFinder, ITempDataMechanism tempDataMechanism)
		{
			this.viewRendererFinder = viewRendererFinder;
			this.tempDataMechanism = tempDataMechanism;
		}

		public override async Task Execute(PipelineContext context, ViewResult result)
		{
			var rendered = new StringBuilder();
			
			var renderer = this.viewRendererFinder.FindViewRenderer(result.ViewType);

			var renderingContext = new ViewRenderingContext
			{
				Writer = new StringWriter(rendered)
			};

			try
			{
				await renderer.Render(result, context, renderingContext);
			}
			catch (Exception e)
			{
				log.Error(e, "Error rendering view `{ViewName}`", result.ViewType.FullName);
#if DEBUG
				if (Debugger.IsAttached) Debugger.Break();
#endif

				// TODO: render something useful
				context.Response.StatusCode = HttpStatusCode.InternalServerError;
				return;
			}

			context.Response.SetCacheHeaders(result);
			context.Response.StatusCode = result.StatusCode;
			context.Response.ContentType = "text/html";

			SetLinkHeaders(context, result);
			this.tempDataMechanism.SetTempData(context, result.TempData);

			await context.Response.Write(rendered.ToString());
		}
	}
}