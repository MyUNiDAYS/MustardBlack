

namespace MustardBlack.Tests.UrlSpecs
{
	public class TrailingDot : Specification
	{
		private Url uri;

		protected override void When()
		{
			this.uri = new Url("http", "www.foo.com", 80, "/some/relative/path.", null);
		}

		[Then]
		public void ShoudlBeNotBeRemoved()
		{
			this.uri.Path.ShouldEqual("/some/relative/path.");
			this.uri.ToString().ShouldEqual("http://www.foo.com/some/relative/path.");
		}
	}
}