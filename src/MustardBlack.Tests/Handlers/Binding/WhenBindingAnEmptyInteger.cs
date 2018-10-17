using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding
{
    public class WhenBindingAnEmptyInteger : BindingSpecification
    {
        private NameValueCollection post;
        private int target;

        protected override void Given()
        {
            base.Given();

            this.post = new NameValueCollection
			       	{
			       		{"numero", ""},
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));

        }

        protected override void When()
        {
            var binder = BinderCollection.FindBinderFor(null, typeof(int), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(int), this.Request, new RouteValues(), false, null);
            this.target = (int)bindingResult.Object;
        }

        [Then]
        public void TheIntShouldBeSetToItsDefaultValue()
        {
	        this.target.ShouldEqual(default(int));
        }
    }
}