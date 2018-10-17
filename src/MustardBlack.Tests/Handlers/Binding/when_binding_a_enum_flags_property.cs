using System;
using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class when_binding_a_enum_flags_property : BindingSpecification
	{
		private NameValueCollection post;
		
		BindingResult bindingResult;

		public class TestClass
		{
			public Modes Modes { get; set; }
		}

		[Flags]
		public enum Modes
		{
			None = 0,
			One = 1,
			Two = 2,
			Four = 4
		}

		protected override void Given()
		{
			base.Given();
			this.post = new NameValueCollection{{"Modes", "1,2,4"}};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
            var binder = BinderCollection.FindBinderFor(null, typeof(TestClass), this.Request, new RouteValues(), null);
            this.bindingResult = binder.Bind("parameter", typeof(TestClass), this.Request, new RouteValues(), false, null);
		}

		[Then]
		public void the_modes_should_be_bound_properly()
		{
			(this.bindingResult.Object as TestClass).Modes.ShouldEqual(Modes.One | Modes.Two | Modes.Four);
		}

		[Then]
		public void report_should_be_valid()
		{
			this.bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
		}
	}
}