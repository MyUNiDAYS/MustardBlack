using System.Threading.Tasks;
using MustardBlack.Applications;
using MustardBlack.Pipeline;

namespace MustardBlack.Hosting.AspNetCore
{
	public static class RequestHandler : Microsoft.AspNetCore.Http.RequestDelegate
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
