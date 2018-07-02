using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
    public class PartQueryString : Specification
    {
        private Url uri;

        protected override void When()
        {
			this.uri = new Url("http", "www.foo.com", 80, "/some/relative/path?KeyOnly", null);
        }

        [Then]
        public void ShoudlToStringProperly()
        {
	        this.uri.ToString().Should().Be("http://www.foo.com/some/relative/path?KeyOnly");
        }

    	[Then]
    	public void ShouldParseQstringProperly()
    	{
		    this.uri.QueryCollection.AllKeys[0].Should().Be(null);
		    this.uri.QueryCollection[null].Should().Be("KeyOnly");
	    }
    }
}