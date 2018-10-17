using System;
using System.Collections.Specialized;
using System.Linq;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class WhenUnSuccessfullyBindingAGuidWithAnInvalidString : BindingSpecification
	{
		private NameValueCollection post;
		private object target;
	    private BindingResult bindingResult;

	    protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			       	{
			       		{"guido", "bad string"},
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

	    protected override void When()
	    {
            var binder = BinderCollection.FindBinderFor("guido", typeof(Guid), this.Request, new RouteValues(), null);
            this.bindingResult = binder.Bind("guido", typeof(Guid), this.Request, new RouteValues(), false, null);
	        this.target = this.bindingResult.Object;
	    }

	    [Then]
	    public void TheGuidShouldBeCorrectlyBoundToAnEmptyGuid()
		{
			this.target.ShouldEqual(Guid.Empty);
		}

        [Then]
        public void AFormatExceptionShouldBeInTheModelErrors()
        {
	        this.bindingResult.BindingErrors.Count().ShouldEqual(1);
	        this.bindingResult.BindingErrors.Single().ErrorKey.ShouldEqual("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).");
        }
	}
}
