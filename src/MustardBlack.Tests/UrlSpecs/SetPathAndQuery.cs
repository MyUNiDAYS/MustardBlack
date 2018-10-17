

namespace MustardBlack.Tests.UrlSpecs
{
    public class SetPathAndQuery : Specification
    {
        Url uri;

	    protected override void Given()
	    {
			this.uri = new Url("http", "www.foo.com", 80, "/some/relative/path?original=querystring");
		}

	    protected override void When()
	    {
		    this.uri.PathAndQuery = "/foo?new=query";
	    }

        [Then]
        public void ShouldToStringProperly()
        {
	        this.uri.ToString().ShouldEqual("http://www.foo.com/foo?new=query");
        }

    	[Then]
    	public void ShouldParseQstringProperly()
	    {
		    this.uri.QueryCollection["new"].ShouldEqual("query");
	    }
    }
}