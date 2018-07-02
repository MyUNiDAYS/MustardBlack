using System;
using FluentAssertions;

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
	        this.uri.ToString().Should().Be("/some/relative/path/");
        }
    }
}
