using System;
using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class when_binding_simple_properties : BindingSpecification
	{
		private NameValueCollection post;
		private InnerClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			{
				{ "boolus", "true,FALSE" },
				{ "stringus", "stringy" },
				{ "guidus", "24F216A2-C331-4374-B4B7-2FF497E41DA5" },
				{ "dateus", "2001/01/01" },
				{ "intus", "20" },
				{ "doublus", "50.5" }
			};

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
		public void all_properties_should_be_correctly_bound()
		{
			this.target.boolus.ShouldEqual(true);
			this.target.guidus.ShouldEqual(new Guid("24F216A2-C331-4374-B4B7-2FF497E41DA5"));
			this.target.stringus.ShouldEqual("stringy");
			this.target.dateus.ShouldEqual(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc));
			this.target.intus.ShouldEqual(20);
			this.target.doublus.ShouldEqual(50.5);
		}
	}
}
