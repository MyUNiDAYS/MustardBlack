using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
	public class HashFragment : Specification
	{
		private Url uri;

		protected override void When()
		{
			this.uri = new Url("http", "www.foo.com", 80, "/some/relative/path.", null, "#foo");
		}

		[Then]
		public void ShoudlBeNotBeRemoved()
		{
			this.uri.Fragment.Should().Be("foo");
			this.uri.Path.Should().Be("/some/relative/path.");
			this.uri.ToString().Should().Be("http://www.foo.com/some/relative/path.#foo");
		}
	}
}