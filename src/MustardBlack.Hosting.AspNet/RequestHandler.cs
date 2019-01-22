using System.Threading.Tasks;
using System.Web;
using MustardBlack.Applications;
using MustardBlack.Pipeline;
using NanoIoC;

namespace MustardBlack.Hosting.AspNet
{
	public sealed class RequestHandler : HttpTaskAsyncHandler
	{
		readonly IContainer container;
		readonly IApplicationRouter applicationRouter;

        public override bool IsReusable => true;

        public RequestHandler()
		{
			this.container = Container.Global;
			this.applicationRouter = this.container.Resolve<IApplicationRouter>();
		}

		public override async Task ProcessRequestAsync(HttpContext httpContext)
		{
			// Create Request
			var request = new AspNetRequest(httpContext);
			this.container.Inject<IRequest>(request, Lifecycle.ExecutionContextLocal);
            
			// Create Response
			var response = new AspNetResponse(httpContext);
			this.container.Inject<IResponse>(response, Lifecycle.ExecutionContextLocal);

			// Create a pipeline context
			var pipelineContext = new PipelineContext(request, response);
			this.container.Inject(pipelineContext, Lifecycle.ExecutionContextLocal);

			// route the application
			await this.applicationRouter.RouteApplication(pipelineContext);
		}
	}
}
