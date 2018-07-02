using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
	public class when_creating_a_url_from_a_valid_urlstring_without_a_query_string : Specification
	{
		private Url url;

		protected override void Given()
		{
			this.url = new Url("http", "www.foo.com", 80, "/foo", null);
		}

		protected override void When()
		{
			
		}

		[Then]
		public void the_path_should_be_correct()
		{
			this.url.Path.Should().Be("/foo");
		}

		[Then]
		public void the_path_and_query_should_be_correct()
		{
			this.url.PathAndQuery.Should().Be("/foo");
		}

		[Then]
		public void the_querystring_should_be_correct()
		{
			this.url.QueryString.Should().Be(string.Empty);
		}


		[Then]
		public void tostring_should_return_the_full_url()
		{
			this.url.ToString().Should().Be("http://www.foo.com/foo");
		}
	}
}