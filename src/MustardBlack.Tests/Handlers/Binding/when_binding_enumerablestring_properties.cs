using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class when_binding_enumerablestring_properties : BindingSpecification
	{
		private NameValueCollection post;
		private InnerClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			       	{
			       		{"strings", "one\ntwo\nthree"},
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
			var strings = this.target.strings.ToArray();

			strings[0].Should().Be("one");
			strings[1].Should().Be("two");
			strings[2].Should().Be("three");
		}
	}
}