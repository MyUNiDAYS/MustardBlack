using MustardBlack.Routing;
using Xunit;

namespace MustardBlack.Tests.Routing.RouteSpecs.GetVirtualPath
{
	public class StaticRoutes
	{
		[Fact]
		public void Root()
		{
			var route = NewRoute("/");
			route.BuildPath(new RouteValues()).ShouldEqual("/");
		}

		[Fact]
		public void SingleSegment()
		{
			var route = NewRoute("/foo");
			route.BuildPath(new RouteValues()).ShouldEqual("/foo");
		}

		[Fact]
		public void MultipleSegments()
		{
			var route = NewRoute("/foo/bar/baz");
			route.BuildPath(new RouteValues()).ShouldEqual("/foo/bar/baz");
		}

		[Fact]
		public void ExtraParameters()
		{
			var route = NewRoute("/foo/bar/baz");
			route.BuildPath(new RouteValues
			{
				{"a", "b"},
				{"c", "d"},
				{"E&", "f%="}
			}).ShouldEqual("/foo/bar/baz");
		}

		static IRoute NewRoute(string url)
		{
			return new Route(url, "testarea", null, false, false);
		}

	}
}