using System;
using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class WhenSuccessfullyBindingAGuid : BindingSpecification
	{
		private NameValueCollection post;
		private dynamic target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			       	{
			       		{"guido", "0BAC2C27-B3A3-4BAD-94A4-9807808EA37C"},
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

	    protected override void When()
	    {
            var binder = BinderCollection.FindBinderFor("guido", typeof(Guid), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("guido", typeof(Guid), this.Request, new RouteValues(), false, null);
	        this.target = bindingResult.Object;
	    }

	    [Then]
	    public void TheGuidShouldBeCorrectlyBound()
	    {
	        bool equality = (this.target.GetType() == (typeof (Guid)));
		    equality.ShouldBeTrue();

		    var equalityGuid = new Guid("0BAC2C27-B3A3-4BAD-94A4-9807808EA37C");
		    equalityGuid.ShouldEqual((Guid)this.target);
	    }
	}
}