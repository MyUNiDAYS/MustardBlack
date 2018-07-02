using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
    public class Equality : Specification
    {
        private Url uri1;
        private Url uri2;
        private Url uri3;

        protected override void When()
        {
			this.uri1 = new Url("https://www.foo.com/bar?baz=boz");
			this.uri2 = new Url("https://www.foo.com/bar/?baz=boz");

			this.uri3 = new Url("https://www.foo.com/bar?baz=bonk");
        }
		
        [Then]
        public void DifferentUrlsShouldNotBeEqual()
        {
	        this.uri1.Should().NotBe(this.uri2);
			this.uri2.Should().NotBe(this.uri3);
            this.uri1.Should().NotBe(this.uri3);
        }
    }
}
