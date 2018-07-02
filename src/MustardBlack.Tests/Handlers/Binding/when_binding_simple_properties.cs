using System;
using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class when_binding_simple_properties : BindingSpecification
	{
		private NameValueCollection post;
		private InnerClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			{
				{ "boolus", "true,FALSE" },
				{ "stringus", "stringy" },
				{ "guidus", "24F216A2-C331-4374-B4B7-2FF497E41DA5" },
				{ "dateus", "2001/01/01" },
				{ "intus", "20" },
				{ "doublus", "50.5" }
			};

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
		public void all_properties_should_be_correctly_bound()
		{
			this.target.boolus.Should().Be(true);
			this.target.guidus.Should().Be(new Guid("24F216A2-C331-4374-B4B7-2FF497E41DA5"));
			this.target.stringus.Should().Be("stringy");
			this.target.dateus.Should().Be(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			this.target.intus.Should().Be(20);
			this.target.doublus.Should().Be(50.5);
		}
	}
}
