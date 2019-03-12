using MustardBlack.Hosting;
using MustardBlack.Routing;
using MustardBlack.Tests.Helpers;
using Xunit;

namespace MustardBlack.Tests.Routing.RouteSpecs.GetRouteData
{
	public class StaticRouteTests
	{
		[Fact]
		public void Root()
		{
			var route = NewRoute("/");

			route.GetRouteValues(TestRequest("/foo")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/"));
			routeData.Count.ShouldEqual(1);
		}

		[Fact]
		public void SingleSegment()
		{
			var route = NewRoute("/foo");

			route.GetRouteValues(TestRequest("/bar")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/foo"));
			routeData.Count.ShouldEqual(1);
		}

		[Fact]
		public void MultipleSegments()
		{
			var route = NewRoute("/foo/bar/baz");

			route.GetRouteValues(TestRequest("/foo")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/foo/bar/baz"));
			routeData.Count.ShouldEqual(1);
		}

		[Fact]
		public void CaseInsensitivity()
		{
			var route = NewRoute("/foo/BAR/BaZ");

			route.GetRouteValues(TestRequest("/foo")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/FOO/bar/bAz"));
			routeData.Count.ShouldEqual(1);
		}


		static IRoute NewRoute(string url)
		{
			return new Route(url, "testarea", null, false, false);
		}

		static IRequest TestRequest(string url)
		{
			var testRequest = new TestRequest { Url = { PathAndQuery = url } };
			return testRequest;
		}
	}
}
