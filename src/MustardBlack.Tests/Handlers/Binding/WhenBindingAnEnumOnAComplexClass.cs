using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding
{
    public sealed class WhenBindingAnEnumOnAComplexClass : BindingSpecification
    {
        private NameValueCollection post;
        private EnumTestViewModel target;

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
            var binder = BinderCollection.FindBinderFor(null, typeof(EnumTestViewModel), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(EnumTestViewModel), this.Request, new RouteValues(), false, null);
            this.target = (EnumTestViewModel)bindingResult.Object;
        }

        [Then]
        public void TheEnumOnTheClassShouldBeBound()
        {
	        this.target.Test.ShouldEqual(EnumTestViewModel.TestingEnum.Test2);
        }
    }

    internal sealed class EnumTestViewModel
    {
        public TestingEnum Test { get; set; }

        internal enum TestingEnum
        {
            Test1 = 1,
            Test2 = 2
        }    
    }

    
}
