using System.Linq;
using MustardBlack.Hosting;

namespace MustardBlack.Tests.Hosting.RequestCookieCollectionSpecs
{
	public sealed class Parsing
	{
		[Then]
		public void SingleCookie()
		{
			var subject = new RequestCookieCollection("name=value");
			subject.Get("name").Value.ShouldEqual("value");
			subject.Count().ShouldEqual(1);
		}

		[Then]
		public void TrailingSeparator()
		{
			var subject = new RequestCookieCollection("name=value;");

			subject.Get("name").Value.ShouldEqual("value");
			subject.Count().ShouldEqual(1);
		}

		[Then]
		public void TwoCookies()
		{
			var subject = new RequestCookieCollection("name=value;name2=value2");

			subject.Get("name").Value.ShouldEqual("value");
			subject.Get("name2").Value.ShouldEqual("value2");
			subject.Count().ShouldEqual(2);
		}

		[Then]
		public void CommaSeparated()
		{
			var subject = new RequestCookieCollection("name=value,name2=value2");

			subject.Get("name").Value.ShouldEqual("value");
			subject.Get("name2").Value.ShouldEqual("value2");
			subject.Count().ShouldEqual(2);
		}

		[Then]
		public void MixedSeparated()
		{
			var subject = new RequestCookieCollection("name=value,name2=value2;name3=value3");

			subject.Get("name").Value.ShouldEqual("value");
			subject.Get("name2").Value.ShouldEqual("value2");
			subject.Get("name3").Value.ShouldEqual("value3");
			subject.Count().ShouldEqual(3);
		}

		[Then]
		public void Whitespace()
		{
			var subject = new RequestCookieCollection("name=value\n;\tname2=value2 ; name3=value3\n");

			subject.Get("name").Value.ShouldEqual("value");
			subject.Get("name2").Value.ShouldEqual("value2");
			subject.Get("name3").Value.ShouldEqual("value3");
			subject.Count().ShouldEqual(3);
		}

		[Then]
		public void CommaInValue()
		{
			var subject = new RequestCookieCollection("name=value,value2");

			subject.Get("name").Value.ShouldEqual("value");
			subject.Get("value2").Value.ShouldEqual("");
			subject.Count().ShouldEqual(2);
		}

		[Then]
		public void Base64Value()
		{
			var cookieValue = "WC4YUK+AAr3CTkapr3SMoiijXl/8H0Ik5vSNyJFemRjihUJZcQ6VbugPfw81HGAOhteaVTEDEGpovCLDL6mnLJlvZheJKHMxkGiYTDqAY3UHfdIJc3H+pmby5diysgBth/kOWVi9dnDx0tTv7dsnt0psbFG2o/iu93vOiIJshqkRcUCwo2cWjtU80gO1zkhnPzgelj4K85do7GI17oZFrSimROIFHTXM";

			var subject = new RequestCookieCollection($"name={cookieValue}");

			subject.Get("name").Value.ShouldEqual(cookieValue);
		}
	}
}
