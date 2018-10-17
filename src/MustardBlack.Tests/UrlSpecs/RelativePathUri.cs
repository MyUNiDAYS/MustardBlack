using System;


namespace MustardBlack.Tests.UrlSpecs
{
    public class RelativePathUri : Specification
    {
        Url uri;

        protected override void When()
        {
			this.uri = new Url(new Uri("/some/relative/path/", UriKind.Relative));
        }

        [Then]
        public void ShoudlToStringProperly()
        {
	        this.uri.ToString().ShouldEqual("/some/relative/path/");
        }
    }
}
