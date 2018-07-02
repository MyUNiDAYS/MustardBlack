using System;
using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class when_binding_nested_simple_properties : BindingSpecification
	{
		private NameValueCollection post;
		private MiddleClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			       	{
			       		{"Inner.boolus", "true,FALSE"},
			       		{"Inner.stringus", "stringy"},
			       		{"Inner.dateus", "2001/01/01"},
			       		{"Inner.intus", "20"},
			       		{"Inner.doublus", "50.5"}
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
            var binder = BinderCollection.FindBinderFor(null, typeof(MiddleClass), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(MiddleClass), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as MiddleClass;
		}

		[Test]
		public void all_properties_should_be_correctly_bound()
		{
			this.target.Inner.boolus.Should().Be(true);
			this.target.Inner.stringus.Should().Be("stringy");
			this.target.Inner.dateus.Should().Be(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			this.target.Inner.intus.Should().Be(20);
			this.target.Inner.doublus.Should().Be(50.5);
		}
	}
}
