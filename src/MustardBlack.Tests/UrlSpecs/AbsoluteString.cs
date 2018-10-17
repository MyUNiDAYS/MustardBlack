

namespace MustardBlack.Tests.UrlSpecs
{
    public class AbsoluteString : Specification
    {
        private Url uri;

        protected override void When()
        {
			this.uri = new Url("http://www.test.com/some/path?query=string");
        }

        [Then]
        public void ShouldToStringProperly()
        {
	        this.uri.ToString().ShouldEqual("http://www.test.com/some/path?query=string");
        }
    }
}