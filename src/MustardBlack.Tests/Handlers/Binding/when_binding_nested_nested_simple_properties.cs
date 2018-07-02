using System;
using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class when_binding_nested_nested_simple_properties : BindingSpecification
	{
		NameValueCollection post;
		OuterClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			{
				{ "Middle.Inner.boolus", "true,FALSE" },
				{ "Middle.Inner.stringus", "stringy" },
				{ "Middle.Inner.dateus", "2001/01/01" },
				{ "Middle.Inner.intus", "20" },
				{ "Middle.Inner.doublus", "50.5" }
			};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
			var binder = BinderCollection.FindBinderFor(null, typeof (OuterClass), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("parameter", typeof (OuterClass), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as OuterClass;
		}

		[Test]
		public void all_properties_should_be_correctly_bound()
		{
			this.target.Middle.Inner.boolus.Should().Be(true);
			this.target.Middle.Inner.stringus.Should().Be("stringy");
			this.target.Middle.Inner.dateus.Should().Be(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			this.target.Middle.Inner.intus.Should().Be(20);
			this.target.Middle.Inner.doublus.Should().Be(50.5);
		}
	}
}
