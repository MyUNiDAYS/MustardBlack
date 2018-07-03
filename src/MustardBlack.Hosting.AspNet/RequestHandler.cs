using System.Threading.Tasks;
using System.Web;
using MustardBlack.Applications;
using MustardBlack.Pipeline;
using NanoIoC;

namespace MustardBlack.Hosting.AspNet
{
	public class RequestHandler : HttpTaskAsyncHandler
	{
		readonly IContainer container;
		readonly IApplicationRouter applicationRouter;

		public RequestHandler()
		{
			this.container = Container.Global;
			this.applicationRouter = this.container.Resolve<IApplicationRouter>();
		}

		public override async Task ProcessRequestAsync(HttpContext httpContext)
		{
			// Create Request
			var request = new AspNetRequest(httpContext);
			this.container.Inject<IRequest>(request, Lifecycle.HttpContextOrExecutionContextLocal);

			// Remove trailing slashes from paths. 
			// TODO: Move this code to a PO
			if (request.Url.Path.Length > 1 && request.Url.Path.EndsWith("/"))
			{
				var newUrl = new Url(request.Url.PathAndQuery);
				newUrl.Path = newUrl.Path.TrimEnd('/');

				httpContext.Response.StatusCode = 301;
				httpContext.Response.Headers.Set("Location", newUrl);
				return;
			}

			// Create Response
			var response = new AspNetResponse(httpContext);
			this.container.Inject<IResponse>(response, Lifecycle.HttpContextOrExecutionContextLocal);

			// Create a pipeline context
			var pipelineContext = new PipelineContext(request, response);
			this.container.Inject(pipelineContext, Lifecycle.HttpContextOrExecutionContextLocal);

			// route the application
			await this.applicationRouter.RouteApplication(pipelineContext);
		}

		public override bool IsReusable { get; } = false;
	}
}
