using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Tests.Routing.RouteSpecs
{
	public static class RouteExtensions
	{
		public static RouteValues GetRouteValues(this IRoute route, IRequest request)
		{
			return route.GetRouteValues(request.Url, request.HttpMethod, request.RequestType());
		}
	}
}