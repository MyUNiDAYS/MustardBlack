using MustardBlack.Hosting;
using MustardBlack.Routing;
using MustardBlack.Tests.Helpers;
using Xunit;

namespace MustardBlack.Tests.Routing.RouteSpecs.GetRouteData
{
	public class LocalisedRouteTests
	{
		[Fact]
		public void SingleSegment()
		{
			var route = NewRoute("/{foo}");
			
			var routeData = route.GetRouteValues(TestRequest("/bar"));
			routeData["foo"].ShouldEqual("bar");

			routeData = route.GetRouteValues(TestRequest("/GB/en-GB/bar"));
			routeData["foo"].ShouldEqual("bar");
		}
		
		static IRoute NewRoute(string url)
		{
			return new Route(url, "testarea", null, true, false);
		}

		static IRequest TestRequest(string url)
		{
			var testRequest = new TestRequest { Url = { PathAndQuery = url } };
			return testRequest;
		}
	}
}
