

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
	        this.uri.ToString().ShouldEqual("/some/relative/path?foo=bar#baz");
        }

        [Then]
        public void HostShouldBeNull()
        {
	        this.uri.Host.ShouldBeNull();
        }
        [Then]
        public void SChemeShouldBeNull()
        {
	        this.uri.Scheme.ShouldBeNull();
        }
    }
}