using System;
using MustardBlack.Routing;
using Xunit;

namespace MustardBlack.Tests.Routing.RouteSpecs
{
	class TestUrl
	{
		public string Url { get; set; }
		public string Label { get; set; }
		public Type ExpectedExceptionType { get; set; }
	}
	
	public class InvalidRoutes
	{
		static readonly TestUrl[] invalidUrls =
		{
			// must start with '/'
			new TestUrl {Url = "foo", ExpectedExceptionType = typeof (ArgumentException), Label = "#2"},

			// cannot contain '?'
			new TestUrl {Url = "/foo?bar", ExpectedExceptionType = typeof (ArgumentException), Label = "#3"},

			// unmatched '{'
			new TestUrl {Url = "/foo/{bar", ExpectedExceptionType = typeof (ArgumentException), Label = "#4"},

			// unmatched '}'
			new TestUrl {Url = "/foo/bar}", ExpectedExceptionType = typeof (ArgumentException), Label = "#5"},

			// "" is an invalid parameter name.
			new TestUrl {Url = "/foo/{}", ExpectedExceptionType = typeof (ArgumentException), Label = "#6"},

			// incomplete parameter in path segment.
			new TestUrl {Url = "/foo/{x/y/z}", ExpectedExceptionType = typeof (ArgumentException), Label = "#7"},

			// regarded as an incomplete parameter
			new TestUrl {Url = "/foo/{a{{b}}c}", ExpectedExceptionType = typeof (ArgumentException), Label = "#8"},

			// consecutive parameters are not allowed
			new TestUrl {Url = "/foo/{a}{b}", ExpectedExceptionType = typeof (ArgumentException), Label = "#9"},

			// consecutive segment separators '/' are not allowed
			new TestUrl {Url = "/foo//bar", ExpectedExceptionType = typeof (ArgumentException), Label = "#10"},

			// A catch-all parameter can only appear as the last segment of the route URL
			new TestUrl {Url = "/{first}/{*rest}/{foo}", ExpectedExceptionType = typeof (ArgumentException), Label = "#11"},

			// A path segment that contains more than one section, such as a literal section or a parameter, cannot contain a catch-all parameter.
			new TestUrl {Url = "/{first}/{*rest}-{foo}", ExpectedExceptionType = typeof (ArgumentException), Label = "#12"},

			// A path segment that contains more than one section, such as a literal section or a parameter, cannot contain a catch-all parameter.
			new TestUrl {Url = "/{first}/{foo}-{*rest}", ExpectedExceptionType = typeof (ArgumentException), Label = "#13"},

			// A path segment that contains more than one section, such as a literal section or a parameter, cannot contain a catch-all parameter.
			new TestUrl {Url = "/-{*rest}", ExpectedExceptionType = typeof (ArgumentException), Label = "#14"},
		};

		[Fact]
		public void InvalidUrls()
		{
			Route r;

			foreach (TestUrl tu in invalidUrls)
			{
				try
				{
					new Route(tu.Url, "testarea", null, false, false);
				}
				catch (Exception e)
				{
					e.ShouldBeInstanceOf(tu.ExpectedExceptionType);
				}
			}
		}
	}
}
