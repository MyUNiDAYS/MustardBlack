using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using Serilog;

namespace MustardBlack.Routing
{
	public sealed class RoutingPipelineOperator : IPreResultPipelineOperator
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public Task<PipelineContinuation> Operate(PipelineContext context)
		{
			var routeData = context.Application.RouteRequest(context.Request.Url, context.Request.HttpMethod, context.Request.RequestType());

			if (routeData != null)
			{
				log.Debug("Routed {url} to {handler} with Method {method} and IsAjax {isAjax}", context.Request.Url, routeData.Route.HandlerType.FullName, context.Request.HttpMethod, context.Request.IsAjaxRequest());

				routeData.HandlerAction = context.Application.HandlerCache.GetHandlerAction(routeData.Route.HandlerType, context.Request.HttpMethod);
				context.RouteData(routeData);
			}
			else
			{
				log.Debug("Could not route {url} to any handler with Method {method} and IsAjax {isAjax}", context.Request.Url, context.Request.HttpMethod, context.Request.IsAjaxRequest());
			}

			return Task.FromResult(PipelineContinuation.Continue);
		}
	}
}
