using System;
using System.Collections.Specialized;
using System.Linq;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NanoIoC;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class WhenBindingNestedSimpleInvalidProperties : BindingSpecification
	{
		private NameValueCollection post;
		private MiddleClass target;
		BindingResult bindingResult;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			       	{
			       		{"Inner.boolus", "to bool or not to bool? not to bool!"},
			       		{"Inner.dateus", "i am not a date!"},
			       		{"Inner.intus", "ha, do i look like an int?!"}
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
            var binder = BinderCollection.FindBinderFor(null, typeof(MiddleClass), this.Request, new RouteValues(), null);
            this.bindingResult = binder.Bind("parameter", typeof(MiddleClass), this.Request, new RouteValues(), false, null);
			this.target = this.bindingResult.Object as MiddleClass;
		}

		[Then]
		public void TheBindingResultShouldHaveErrors()
		{
			var bindingErrors = this.bindingResult.BindingErrors.ToArray();

			bindingErrors[0].ParameterKey.ShouldEqual("Inner.dateus");
			bindingErrors[0].ErrorKey.ShouldEqual("Invalid");
			bindingErrors[0].AttemptedValue.ShouldEqual("i am not a date!");


			bindingErrors[1].ParameterKey.ShouldEqual("Inner.intus");
			bindingErrors[1].ErrorKey.ShouldEqual("Invalid");
			bindingErrors[1].AttemptedValue.ShouldEqual("ha, do i look like an int?!");
		}

		[Then]
		public void AllPropertiesShouldHaveDefaultValues()
		{
			this.target.Inner.boolus.ShouldEqual(false);
			this.target.Inner.dateus.ShouldEqual(DateTime.MinValue);
			this.target.Inner.intus.ShouldEqual(0);
		}
	}
}
