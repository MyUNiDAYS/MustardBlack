using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MustardBlack.Applications;
using MustardBlack.Hosting;
using MustardBlack.Hosting.AspNetCore;
using MustardBlack.Pipeline;
using NanoIoC;

namespace MustardBlack.Example.AspNetCore
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			var container = Container.Global;
			app.Run(async context =>
			{
				// Create Request
				var request = new AspNetCoreRequest(context);
				container.Inject<IRequest>(request, ServiceLifetime.Scoped);

				// Create Response
				var response = new AspNetCoreResponse(context);
				container.Inject<IResponse>(response, ServiceLifetime.Scoped);

				// Create a pipeline context
				var pipelineContext = new PipelineContext(request, response);
				container.Inject(pipelineContext, ServiceLifetime.Scoped);

				var applicationRouter = container.Resolve<IApplicationRouter>();
				// route the application
				await applicationRouter.RouteApplication(pipelineContext);

				await context.Response.WriteAsync("Hello World!");
			});
		}
	}
}
