
using MustardBlack.Routing;

namespace MustardBlack.Tests.Routing.RouteValueSpecs.CaseInsensitivity
{
	public class Add : Specification
	{
		RouteValues subject;

		protected override void Given()
		{
			this.subject = new RouteValues();
		}

		protected override void When()
		{
			this.subject.Add("FOO", "bar");
		}

		[Then]
		public void ShouldContainKeyCaseInsensitively()
		{
			this.subject.ContainsKey("foo").ShouldBeTrue();
		}
	}
}
