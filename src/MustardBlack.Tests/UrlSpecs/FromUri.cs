using System;
using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
    public class FromUri : Specification
    {
        private Url uri;

        protected override void When()
        {
			this.uri = new Url(new Uri("https://www.myunidays.com/some/path/?key=value#fragment"));
        }

        [Then]
        public void ShoudlToStringProperly()
        {
	        this.uri.ToString().Should().Be("https://www.myunidays.com/some/path/?key=value#fragment");
        }
    }
}
