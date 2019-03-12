using MustardBlack.Routing;
using Xunit;

namespace MustardBlack.Tests.Routing.RouteSpecs.GetVirtualPath
{
	public class DynamicRoutes
	{
		[Fact]
		public void SingleSegment()
		{
			NewRoute("/{foo}").BuildPath(new RouteValues(new {FOO = "bar"})).ShouldEqual("/bar");
		}

		[Fact]
		public void EmptyParameterName()
		{
			NewRoute("/{ }").BuildPath(new RouteValues { {" ", "bar"}}).ShouldEqual("/bar");
		}

		[Fact]
		public void MultipleSegments()
		{
			NewRoute("/{foo}/{bar}").BuildPath(new RouteValues(new
			{
				FOO = "bar",
				bar = "baz"
			})).ShouldEqual("/bar/baz");
		}

		[Fact]
		public void TypeConversion()
		{
			NewRoute("/{foo}/{bar}").BuildPath(new RouteValues(new
			{
				FOO = "baz",
				bar = 1
			})).ShouldEqual("/baz/1");
		}

		[Fact]
		public void WithExtensions()
		{
			NewRoute("/{foo}.txt").BuildPath(new RouteValues(new
			{
				foo = "bar"
			})).ShouldEqual("/bar.txt");
		}

		[Fact]
		public void WithLiteralSegments()
		{
			NewRoute("/{foo}/baz").BuildPath(new RouteValues(new
			{
				foo = "bar"
			})).ShouldEqual("/bar/baz");
		}

		[Fact]
		public void MixedLiteralSegments()
		{
			NewRoute("/{foo}.txt/{baz}").BuildPath(new RouteValues(new
			{
				foo = "bar",
				baz = "bingo"
			})).ShouldEqual("/bar.txt/bingo");
		}

		[Fact]
		public void MissingParameters()
		{
			NewRoute("/{foo}/{baz}").BuildPath(new RouteValues(new
			{
				foo = "bar",
			})).ShouldBeNull();
		}

		static IRoute NewRoute(string url)
		{
			return new Route(url, "testarea", null, false, false);
		}

	}
}