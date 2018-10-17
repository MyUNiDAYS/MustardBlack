using System;
using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class WhenBindingSimpleMissingProperties : BindingSpecification
	{
		private NameValueCollection post;
		private InnerClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection ();

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
            var binder = BinderCollection.FindBinderFor(null, typeof(InnerClass), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(InnerClass), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as InnerClass;
		}

		[Then]
		public void AllPropertiesShouldHaveDefaultValues()
		{
			this.target.boolus.ShouldEqual(false);
			this.target.guidus.ShouldEqual(Guid.Empty);
			this.target.stringus.ShouldEqual(null);
			this.target.dateus.ShouldEqual(DateTime.MinValue);
			this.target.intus.ShouldEqual(0);
			this.target.doublus.ShouldEqual(0.0);
		}
	}
}
