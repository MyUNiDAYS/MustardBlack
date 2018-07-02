using FluentAssertions;

namespace MustardBlack.Tests.UrlSpecs
{
	public class when_creating_a_url_with_data : Specification
	{
		private Url url;

		protected override void Given()
		{
			this.url = new Url("http", "www.foo.com", 80, "/foo?x=y&p=q", null);
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
			this.url.PathAndQuery.Should().Be("/foo?p=q&x=y");
		}


		[Then]
		public void the_querystring_should_be_correct()
		{
			this.url.QueryString.Should().Be("?p=q&x=y");
		}

		[Then]
		public void the_querycollection_should_contain_the_correct_params()
		{
			this.url.QueryCollection.Count.Should().Be(2);
			this.url.QueryCollection["x"].Should().Be("y");
			this.url.QueryCollection["p"].Should().Be("q");
		}

		[Then]
		public void tostring_should_return_the_full_url()
		{
			this.url.ToString().Should().Be("http://www.foo.com/foo?p=q&x=y");
		}
	}
}