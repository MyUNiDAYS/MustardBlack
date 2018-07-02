using System;
using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NanoIoC;
using NSubstitute;
using NUnit.Framework;

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

			bindingErrors[0].ParameterKey.Should().Be("Inner.dateus");
			bindingErrors[0].ErrorKey.Should().Be("Invalid");
			bindingErrors[0].AttemptedValue.Should().Be("i am not a date!");


			bindingErrors[1].ParameterKey.Should().Be("Inner.intus");
			bindingErrors[1].ErrorKey.Should().Be("Invalid");
			bindingErrors[1].AttemptedValue.Should().Be("ha, do i look like an int?!");
		}

		[Test]
		public void AllPropertiesShouldHaveDefaultValues()
		{
			this.target.Inner.boolus.Should().Be(false);
			this.target.Inner.dateus.Should().Be(DateTime.MinValue);
			this.target.Inner.intus.Should().Be(0);
		}
	}
}
