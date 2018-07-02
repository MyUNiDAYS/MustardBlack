using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
    public class RelativePathWithDataAndFragment : Specification
    {
        private Url uri;

        protected override void When()
        {
			this.uri = new Url("/some/relative/path?foo=bar#baz");
        }

        [Then]
        public void ShoudlToStringProperly()
        {
	        this.uri.ToString().Should().Be("/some/relative/path?foo=bar#baz");
        }

        [Then]
        public void HostShouldBeNull()
        {
	        this.uri.Host.Should().BeNull();
        }
        [Then]
        public void SChemeShouldBeNull()
        {
	        this.uri.Scheme.Should().BeNull();
        }
    }
}