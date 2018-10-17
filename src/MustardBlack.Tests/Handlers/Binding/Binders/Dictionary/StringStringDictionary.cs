using System.Collections.Generic;
using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Dictionary
{
	public class StringStringDictionary : BindingSpecification
	{
		NameValueCollection post;
		TestResource target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
					   {
						   {"dict[keyA]", "valueA"},
						   {"dict[keyB]", "valueB"},
						   {"dict[keyC]", "valueC"},
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
			this.target.dict["keyA"].ShouldEqual("valueA");
			this.target.dict["keyB"].ShouldEqual("valueB");
			this.target.dict["keyC"].ShouldEqual("valueC");
		}

		class TestResource
		{
			public Dictionary<string,string> dict { get; set; }
		}
	}
}
