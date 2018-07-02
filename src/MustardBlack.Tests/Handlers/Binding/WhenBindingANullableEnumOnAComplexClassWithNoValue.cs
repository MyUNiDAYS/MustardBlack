using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
    public sealed class WhenBindingANullableEnumOnAComplexClassWithNoValue : BindingSpecification
    {
        private NameValueCollection post;
        private NullableNoValueEnumTestViewModel target;

        protected override void Given()
        {
            base.Given();

            this.post = new NameValueCollection
                            {
                                {"Test", ""}
                            };

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
        }

        protected override void When()
        {
            var binder = BinderCollection.FindBinderFor(null, typeof(NullableNoValueEnumTestViewModel), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(NullableNoValueEnumTestViewModel), this.Request, new RouteValues(), false, null);
            this.target = (NullableNoValueEnumTestViewModel)bindingResult.Object;
        }

        [Test]
        public void TheEnumOnTheClassShouldBeNull()
        {
	        this.target.Test.HasValue.Should().BeFalse();
        }

        internal sealed class NullableNoValueEnumTestViewModel
        {
            public TestingEnum? Test { get; set; }

            internal enum TestingEnum
            {
                Test1 = 1,
                Test2 = 2
            }
        }
    }
}