using System;
using System.Collections.Specialized;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class WhenUnSuccessfullyBindingAGuidWithAnEmptyString : BindingSpecification
	{
		private NameValueCollection post;
		private object target;
	    private BindingResult bindingResult;

	    protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			       	{
			       		{"guido", ""},
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
        public void AFormatExceptionShouldNotBeInTheModelErrors()
        {
	        this.bindingResult.BindingErrors.ShouldBeEmpty();
        }
	}
}
