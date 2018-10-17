using System.Collections.Specialized;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Bools
{
    public class WhenBindingAnEmptyNullableBool : BindingSpecification
    {
        private NameValueCollection post;
        private bool? target;

        protected override void Given()
        {
            base.Given();

            this.post = new NameValueCollection
			       	{
			       		{"boolus", ""},
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));

        }

        protected override void When()
        {
			var binder = BinderCollection.FindBinderFor("boolus", typeof(bool?), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("boolus", typeof(bool?), this.Request, new RouteValues(), false, null);
            this.target = (bool?)bindingResult.Object;
        }

        [Then]
        public void TheValueShouldBeNull()
        {
            this.target.ShouldBeNull();
        }
    }
}
