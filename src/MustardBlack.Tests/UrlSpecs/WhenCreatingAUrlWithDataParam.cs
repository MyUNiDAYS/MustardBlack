

namespace MustardBlack.Tests.UrlSpecs
{
	public class WhenCreatingAUrlWithDataParam : Specification
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
			this.url.Path.ShouldEqual("/foo");
		}

   
		[Then]
		public void the_path_and_query_should_be_correct()
		{
			this.url.PathAndQuery.ShouldEqual("/foo?p=q&x=y");
		}


		[Then]
		public void the_querystring_should_be_correct()
		{
			this.url.QueryString.ShouldEqual("?p=q&x=y");
		}

		[Then]
		public void the_querycollection_should_contain_the_correct_params()
		{
			this.url.QueryCollection.Count.ShouldEqual(2);
			this.url.QueryCollection["x"].ShouldEqual("y");
			this.url.QueryCollection["p"].ShouldEqual("q");
		}

		[Then]
		public void tostring_should_return_the_full_url()
		{
			this.url.ToString().ShouldEqual("http://www.foo.com/foo?p=q&x=y");
		}
	}
}