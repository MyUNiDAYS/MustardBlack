using System.Linq;
using FluentAssertions;
using MustardBlack.Hosting;
using NUnit.Framework;

namespace MustardBlack.Tests.Hosting.RequestCookieCollectionSpecs
{
	[TestFixture]
	sealed class Parsing
	{
		[Test]
		public void SingleCookie()
		{
			var subject = new RequestCookieCollection("name=value");
			subject.Get("name").Value.Should().Be("value");
			subject.Count().Should().Be(1);
		}

		[Test]
		public void TrailingSeparator()
		{
			var subject = new RequestCookieCollection("name=value;");

			subject.Get("name").Value.Should().Be("value");
			subject.Count().Should().Be(1);
		}

		[Test]
		public void TwoCookies()
		{
			var subject = new RequestCookieCollection("name=value;name2=value2");

			subject.Get("name").Value.Should().Be("value");
			subject.Get("name2").Value.Should().Be("value2");
			subject.Count().Should().Be(2);
		}

		[Test]
		public void CommaSeparated()
		{
			var subject = new RequestCookieCollection("name=value,name2=value2");

			subject.Get("name").Value.Should().Be("value");
			subject.Get("name2").Value.Should().Be("value2");
			subject.Count().Should().Be(2);
		}

		[Test]
		public void MixedSeparated()
		{
			var subject = new RequestCookieCollection("name=value,name2=value2;name3=value3");

			subject.Get("name").Value.Should().Be("value");
			subject.Get("name2").Value.Should().Be("value2");
			subject.Get("name3").Value.Should().Be("value3");
			subject.Count().Should().Be(3);
		}

		[Test]
		public void Whitespace()
		{
			var subject = new RequestCookieCollection("name=value\n;\tname2=value2 ; name3=value3\n");

			subject.Get("name").Value.Should().Be("value");
			subject.Get("name2").Value.Should().Be("value2");
			subject.Get("name3").Value.Should().Be("value3");
			subject.Count().Should().Be(3);
		}

		[Test]
		public void CommaInValue()
		{
			var subject = new RequestCookieCollection("name=value,value2");

			subject.Get("name").Value.Should().Be("value");
			subject.Get("value2").Value.Should().Be("");
			subject.Count().Should().Be(2);
		}

		[Test]
		public void Base64Value()
		{
			var cookieValue = "WC4YUK+AAr3CTkapr3SMoiijXl/8H0Ik5vSNyJFemRjihUJZcQ6VbugPfw81HGAOhteaVTEDEGpovCLDL6mnLJlvZheJKHMxkGiYTDqAY3UHfdIJc3H+pmby5diysgBth/kOWVi9dnDx0tTv7dsnt0psbFG2o/iu93vOiIJshqkRcUCwo2cWjtU80gO1zkhnPzgelj4K85do7GI17oZFrSimROIFHTXM";

			var subject = new RequestCookieCollection($"name={cookieValue}");

			subject.Get("name").Value.Should().Be(cookieValue);
		}
	}
}
