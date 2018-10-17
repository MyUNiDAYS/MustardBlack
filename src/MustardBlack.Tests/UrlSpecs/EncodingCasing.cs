namespace MustardBlack.Tests.UrlSpecs
{
	public class EncodingCasing : Specification
	{
		private Url uri;

		protected override void When()
		{
			this.uri = new Url("http://www.test.com/SOME/path");
			this.uri.QueryCollection["k/y"] = ":";
		}

		[Then]
		public void ShouldToStringProperly()
		{
			this.uri.ToString().ShouldEqual("http://www.test.com/SOME/path?k%2fy=%3a");
		}
	}
}