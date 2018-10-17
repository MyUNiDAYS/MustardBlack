using System.Collections.Generic;
using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Dictionary
{
	public class UsingComplexTypeAsValue : BindingSpecification
	{
		NameValueCollection post;
		TestResource target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
					   {
						   {"dict[keyA].Foo", "fooA"},
						   {"dict[keyA].Bar", "barA"},
						   {"dict[keyB].Foo", "fooB"},
						   {"dict[keyB].Bar", "barB"},
						   {"dict[keyC].Foo", "fooC"},
						   {"dict[keyC].Bar", "barC"},
					   };

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));
		}

		protected override void When()
		{
			var binder = BinderCollection.FindBinderFor(null, typeof(TestResource), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("parameter", typeof(TestResource), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as TestResource;
		}

		[Then]
		public void BindsCorrectly()
		{
			this.target.dict.ShouldNotBeNull();
			this.target.dict.Count.ShouldEqual(3);
			this.target.dict["keyA"].Foo.ShouldEqual("fooA");
			this.target.dict["keyA"].Bar.ShouldEqual("barA");
			this.target.dict["keyB"].Foo.ShouldEqual("fooB");
			this.target.dict["keyB"].Bar.ShouldEqual("barB");
			this.target.dict["keyC"].Foo.ShouldEqual("fooC");
			this.target.dict["keyC"].Bar.ShouldEqual("barC");
		}

		class TestResource
		{
			public Dictionary<string, InnerType> dict { get; set; }

			public class InnerType
			{
				public string Foo { get; set; }
				public string Bar { get; set; }
			}
		}
	}
}
