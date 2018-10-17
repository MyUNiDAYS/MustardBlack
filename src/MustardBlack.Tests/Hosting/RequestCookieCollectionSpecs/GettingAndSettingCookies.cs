
using MustardBlack.Hosting;

namespace MustardBlack.Tests.Hosting.RequestCookieCollectionSpecs
{
	public sealed class GettingAndSettingCookies : Specification
	{
		RequestCookieCollection subject;

		protected override void Given()
		{
			this.subject = new RequestCookieCollection("name=value");
		}

		protected override void When()
		{
			// replace parsed
			this.subject.Set(new RequestCookie("name", "new value"));

			// double set replace
			this.subject.Set(new RequestCookie("name2", "new value 2"));
			this.subject.Set(new RequestCookie("name2", "new value 3"));

			// new add
			this.subject.Set(new RequestCookie("name3", "new value 3"));
		}

		[Then]
		public void ParsedReplacentShouldContainNewValue()
		{
			this.subject.Get("name").Value.ShouldEqual("new value");
		}
	
		[Then]
		public void DoubleAdditionShouldContainLatestValue()
		{
			this.subject.Get("name2").Value.ShouldEqual("new value 3");
		}

		[Then]
		public void NewAdditionShouldContainNewValue()
		{
			this.subject.Get("name3").Value.ShouldEqual("new value 3");
		}

		[Then]
		public void NonExistantShouldReturnNull()
		{
			this.subject.Get("i-dont-exist").ShouldBeNull();
		}
	}
}
