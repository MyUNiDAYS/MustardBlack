

namespace MustardBlack.Tests.UrlSpecs
{
	public class when_modifying_the_query_collection : Specification
	{
		private Url url;

		protected override void Given()
		{
			this.url = new Url("http", "www.foo.com", 80, "/?a=b&b=c", null);
		}

		protected override void When()
		{
			this.url.QueryCollection.Remove("a");
			this.url.QueryCollection["b"] = "x";
			this.url.QueryCollection["c"] = "y";
			this.url.QueryCollection.Add("d", "z");
		}

		[Then]
		public void the_resulting_url_should_have_the_correct_querystring()
		{
			this.url.ToString().ShouldEqual("http://www.foo.com/?b=x&c=y&d=z");
		}
	}
}