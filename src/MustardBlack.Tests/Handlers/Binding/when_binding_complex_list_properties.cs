using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class when_binding_complex_list_properties : BindingSpecification
	{
		private NameValueCollection post;
		private OuterClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			       	{
			       		{"Middles[0].Something", "zero"},
			       		{"Middles[2].Something", "two"},
			       		{"Middles[3].Something", "three"},
			       		{"Middles[4].Something", "four"}
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
            var binder = BinderCollection.FindBinderFor(null, typeof(OuterClass), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(OuterClass), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as OuterClass;
		}

		[Test]
		public void all_properties_should_be_correctly_bound()
		{
			var middles = this.target.Middles.ToArray();

			middles[0].Something.Should().Be("zero");
			middles[2].Something.Should().Be("two");
			middles[3].Something.Should().Be("three");
			middles[4].Something.Should().Be("four");
		}
	}
}