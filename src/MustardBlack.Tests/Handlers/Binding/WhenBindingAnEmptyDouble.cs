using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
    public class WhenBindingAnEmptyDouble : BindingSpecification
    {
        private NameValueCollection post;
        private double target;

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
            var binder = BinderCollection.FindBinderFor(null, typeof(double), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(double), this.Request, new RouteValues(), false, null);
            this.target = (double)bindingResult.Object;
        }

        [Test]
        public void TheIntShouldBeSetToItsDefaultValue()
        {
	        this.target.Should().Be(default(double));
        }
    }
}
