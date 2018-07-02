using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
    public class Https : Specification
    {
        private Url uri;

        protected override void When()
        {
			this.uri = new Url("https", "www.foo.com", 443, string.Empty);
        }

        [Then]
        public void ShoudlBeRemoved()
        {
	        this.uri.ToString().Should().Be("https://www.foo.com/");
        }
    }
}