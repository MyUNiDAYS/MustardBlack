using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
    public class RelativeUriWithData : Specification
    {
        private Url uri;

        protected override void When()
        {
			this.uri = new Url("http", "www.foo.com", 80, "/some/relative/path/?foo=bar");
        }

        [Then]
        public void ShoudlToStringProperly()
        {
            var newUri = this.uri.ToString();
	        newUri.Should().Be("http://www.foo.com/some/relative/path/?foo=bar");
        }

        [Then]
        public void ShouldHaveCorrectPathAndData()
        {
	        this.uri.Path.Should().Be("/some/relative/path/");
	        this.uri.QueryCollection["foo"].Should().Be("bar");
        }
    }
}
