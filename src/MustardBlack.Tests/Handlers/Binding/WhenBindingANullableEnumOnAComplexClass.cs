using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding
{
    public sealed class WhenBindingANullableEnumOnAComplexClass : BindingSpecification
    {
        private NameValueCollection post;
        private NullableEnumTestViewModel target;

        protected override void Given()
        {
            base.Given();

            this.post = new NameValueCollection
                            {
                                {"Test", "2"}
                            };

            this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
            this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
        }

        protected override void When()
        {
            var binder = BinderCollection.FindBinderFor(null, typeof(NullableEnumTestViewModel), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(NullableEnumTestViewModel), this.Request, new RouteValues(), false, null);
            this.target = (NullableEnumTestViewModel)bindingResult.Object;
        }

        [Then]
        public void TheEnumOnTheClassShouldBeBound()
        {
	        this.target.Test.HasValue.ShouldBeTrue();
        }

        [Then]
        public void TheEnumOnTheClassShouldHaveTheCorrectValue()
        {
	        this.target.Test.ShouldEqual((NullableEnumTestViewModel.TestingEnum?)NullableEnumTestViewModel.TestingEnum.Test2);
        }

        internal sealed class NullableEnumTestViewModel
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