

namespace MustardBlack.Tests.UrlSpecs
{
    public class RelativeUriWithDataSeparate : Specification
    {
        private Url uri;

        protected override void When()
        {
			this.uri = new Url("http", "www.foo.com", 80, "/some/relative/path/");
        }

        [Then]
        public void ShoudlToStringProperly()
        {
            var newUri = this.uri.ToString();
	        newUri.ShouldEqual("http://www.foo.com/some/relative/path/");
        }

        [Then]
        public void ShouldHaveCorrectPathAndData()
        {
	        this.uri.Path.ShouldEqual("/some/relative/path/");
        }
    }
}
