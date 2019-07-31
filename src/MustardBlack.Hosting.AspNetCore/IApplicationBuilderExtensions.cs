using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MustardBlack.Applications;
using MustardBlack.Extensions;
using MustardBlack.Pipeline;
using NanoIoC;

namespace MustardBlack.Hosting.AspNetCore
{
	public static class IApplicationBuilderExtensions
	{
		public static void UseMustardBlack(this IApplicationBuilder app, string rootPath)
		{
			var container = Container.Global;
			
			container.Register<IFileSystem>(c => new NetStandardFileSystem(rootPath));

			try
			{
				var bootstrapperType = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(a => a.GetTypes())
					.Where(t => !t.IsAbstract)
					.FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IBootstrapper)));

				if (bootstrapperType != null)
				{
					var bootstrapper = Activator.CreateInstance(bootstrapperType) as IBootstrapper;
					bootstrapper.Bootstrap();
				}
			}
			catch (TypeInitializationException e)
			{
				throw e.CreateDetailedException();
			}
			catch (ReflectionTypeLoadException e)
			{
				throw e.CreateDetailedException();
			}

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

				context.Response.OnStarting(() =>
				{
					foreach (var key in response.Headers.AllKeys)
						context.Response.Headers[key] = response.Headers[key];

					return Task.CompletedTask;
				});

				// route the application
				await applicationRouter.RouteApplication(pipelineContext);
			});
		}
	}
}