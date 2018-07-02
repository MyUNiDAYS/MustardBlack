using System;
using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

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

	    [Test]
	    public void TheGuidShouldBeCorrectlyBoundToAnEmptyGuid()
		{
			this.target.Should().BeOfType<Guid>().Subject.Should().Be(Guid.Empty);
		}

        [Test]
        public void AFormatExceptionShouldBeInTheModelErrors()
        {
	        this.bindingResult.BindingErrors.Should().HaveCount(1);
	        this.bindingResult.BindingErrors.Single().ErrorKey.Should().Be("Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).");
        }
	}
}
