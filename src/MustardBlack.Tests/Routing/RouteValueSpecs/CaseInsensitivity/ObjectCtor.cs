using FluentAssertions;
using MustardBlack.Routing;

namespace MustardBlack.Tests.Routing.RouteValueSpecs.CaseInsensitivity
{
	public class ObjectCtor : Specification
	{
		RouteValues subject;

		protected override void Given()
		{
			this.subject = new RouteValues(new { FOO = "bar" });
		}

		protected override void When()
		{
		}

		[Then]
		public void ShouldContainKeyCaseInsensitively()
		{
			this.subject.ContainsKey("foo").Should().BeTrue();
		}
	}
}
