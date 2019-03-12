using MustardBlack.Hosting;
using MustardBlack.Routing;
using MustardBlack.Tests.Helpers;
using Xunit;

namespace MustardBlack.Tests.Routing.RouteSpecs.GetRouteData
{
	public class DynamicRouteTests
	{
		[Fact]
		public void SingleSegment()
		{
			var route = NewRoute("/{foo}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/bar"));
			routeData["foo"].ShouldEqual("bar");
		}

		[Fact]
		public void EmptyParameterName()
		{
			var route = NewRoute("/{ }");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/bar"));
			routeData[" "].ShouldEqual("bar");
		}

		[Fact]
		public void MultipleSegments()
		{
			var route = NewRoute("/{foo}/{bar}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/baz")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/far/baz"));
			routeData["foo"].ShouldEqual("far");
			routeData["bar"].ShouldEqual("baz");
		}

		[Fact]
		public void NoTypeConversion()
		{
			var route = NewRoute("/{foo}/{bar}");

			var routeData = route.GetRouteValues(TestRequest("/far/1"));
			routeData["foo"].ShouldEqual("far");
			routeData["bar"].ShouldEqual("1");
			routeData["bar"].ShouldBeInstanceOf<string>();
		}

		[Fact]
		public void MultipleSlashes()
		{
			var route = NewRoute("/{foo}/{bar}");

			route.GetRouteValues(TestRequest("//far/baz")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/far//baz")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/far//baz//boz")).ShouldBeNull();
		}

		[Fact]
		public void CaseInsensitivity()
		{
			var route = NewRoute("/{FOO}/{bAr}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/baz")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/fAr/BaZ"));
			routeData["foo"].ShouldEqual("fAr");
			routeData["bar"].ShouldEqual("BaZ");
		}

		[Fact]
		public void WithExtensions()
		{
			var route = NewRoute("/foo/{bar}.txt");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/.txt")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/baz/.txt")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/baz.txt")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/foo/baz.txt"));
			routeData["bar"].ShouldEqual("baz");
		}

		[Fact]
		public void WithLiteralSegments()
		{
			var route = NewRoute("/foo/{bar}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/baz/bar")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/baz")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/foo/baz"));
			routeData["bar"].ShouldEqual("baz");
		}

		[Fact]
		public void MixedLiteralSegments()
		{
			var route = NewRoute("/foo{bar}/{baz}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/baz/bar")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/baz")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/fooboz/bingo"));
			routeData["bar"].ShouldEqual("boz");
			routeData["baz"].ShouldEqual("bingo");
		}

		[Fact]
		public void GreedyMixedLiteralSegments()
		{
			var route = NewRoute("/{a}.{c}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/a.")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/.c")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/a.c/foo")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/a.b.c"));
			routeData["a"].ShouldEqual("a.b");
			routeData["c"].ShouldEqual("c");
		}
		
		[Fact(Skip = "This is expected to fail. Would be nice if it didnt")]
		public void RepetitiveMixedLiteralSegments()
		{
			var route = NewRoute("/{abc}def{ghi}");
			
			var routeData = route.GetRouteValues(TestRequest("/1defdefdef2"));
			routeData["abc"].ShouldEqual("1def");
			routeData["ghi"].ShouldEqual("def2");
		}

		[Fact]
		public void RepetitiveMixedLiteralSegments2()
		{
			var route = NewRoute("/{abc}def{ghi}");
			
			var routeData = route.GetRouteValues(TestRequest("/1defdeffoo"));
			routeData["abc"].ShouldEqual("1def");
			routeData["ghi"].ShouldEqual("foo");
		}

		[Fact]
		public void MultipleMixedLiteralSegments()
		{
			var route = NewRoute("/{a}/{b}-{c}/{d}/e");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/a/b-c/d")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/a/b-c/e")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/a/b-c/d/e"));
			routeData["a"].ShouldEqual("a");
			routeData["b"].ShouldEqual("b");
			routeData["c"].ShouldEqual("c");
			routeData["d"].ShouldEqual("d");
		}

		[Fact]
		public void CatchallMixedLiteralSegments()
		{
			var route = NewRoute("/foo{bar}/{*baz}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/baz/bar")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/baz")).ShouldBeNull();

			var routeData = route.GetRouteValues(TestRequest("/fooboz/bingo/bongo"));
			routeData["bar"].ShouldEqual("boz");
			routeData["baz"].ShouldEqual("bingo/bongo");
		}

		[Fact]
		public void CatchallRoot()
		{
			var route = NewRoute("/{*baz}");

			route.GetRouteValues(TestRequest("/")).ShouldBeNull();
			route.GetRouteValues(TestRequest("/baz/bar")).ShouldNotBeNull();
			route.GetRouteValues(TestRequest("/foo/bar/baz")).ShouldNotBeNull();

			var routeData = route.GetRouteValues(TestRequest("/fooboz/bingo/bongo"));
			routeData["baz"].ShouldEqual("fooboz/bingo/bongo");
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
