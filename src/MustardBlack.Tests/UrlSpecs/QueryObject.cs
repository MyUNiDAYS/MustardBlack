using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
    public class QueryObject : Specification
    {
        private Url uri;

        protected override void When()
        {
	        string nullString = null;

			this.uri = new Url("http", "www.foo.com", 80, "/some/relative/path?KeyOnly", new {
				number = 1,
				text = "hello",
				nullString = nullString
			});
        }

        [Then]
        public void ShouldToStringProperly()
        {
	        this.uri.ToString().Should().Be("http://www.foo.com/some/relative/path?KeyOnly&nullString=&number=1&text=hello");
        }

    	[Then]
    	public void ShouldParseQstringProperly()
    	{
		    this.uri.QueryCollection[null].Should().Be("KeyOnly");
		    this.uri.QueryCollection["number"].Should().Be("1");
		    this.uri.QueryCollection["text"].Should().Be("hello");
		    this.uri.QueryCollection["nullString"].Should().Be(null);
	    }
    }
}