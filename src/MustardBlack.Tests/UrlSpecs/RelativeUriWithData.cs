

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
	        newUri.ShouldEqual("http://www.foo.com/some/relative/path/?foo=bar");
        }

        [Then]
        public void ShouldHaveCorrectPathAndData()
        {
	        this.uri.Path.ShouldEqual("/some/relative/path/");
	        this.uri.QueryCollection["foo"].ShouldEqual("bar");
        }
    }
}
