

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
	        this.uri.ToString().ShouldEqual("http://www.foo.com/some/relative/path?KeyOnly&nullString=&number=1&text=hello");
        }

    	[Then]
    	public void ShouldParseQstringProperly()
    	{
		    this.uri.QueryCollection[null].ShouldEqual("KeyOnly");
		    this.uri.QueryCollection["number"].ShouldEqual("1");
		    this.uri.QueryCollection["text"].ShouldEqual("hello");
		    this.uri.QueryCollection["nullString"].ShouldEqual(null);
	    }
    }
}