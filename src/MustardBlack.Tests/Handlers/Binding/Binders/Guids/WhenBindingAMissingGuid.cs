using System;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Guids
{
	public class WhenBindingAMissingGuid : BindingSpecification
	{
		Guid target;

		protected override void Given()
		{
			base.Given();

			this.Request.Url.Returns(new Url("http://www.mydomain.com/path"));
		}

		protected override void When()
		{
			var binder = BinderCollection.FindBinderFor("loc", typeof(Guid), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("loc", typeof(Guid), this.Request, new RouteValues(), false, null);
			this.target = (Guid)bindingResult.Object;
		}

		[Test]
		public void TheValueShouldBeEmpty()
		{
			this.target.Should().Be(Guid.Empty);
		}
	}
}
