using System;
using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class WhenBindingSimpleMissingProperties : BindingSpecification
	{
		private NameValueCollection post;
		private InnerClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection ();

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
            var binder = BinderCollection.FindBinderFor(null, typeof(InnerClass), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(InnerClass), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as InnerClass;
		}

		[Test]
		public void AllPropertiesShouldHaveDefaultValues()
		{
			this.target.boolus.Should().Be(false);
			this.target.guidus.Should().Be(Guid.Empty);
			this.target.stringus.Should().Be(null);
			this.target.dateus.Should().Be(DateTime.MinValue);
			this.target.intus.Should().Be(0);
			this.target.doublus.Should().Be(0.0);
		}
	}
}
