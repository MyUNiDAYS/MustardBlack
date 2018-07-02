using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

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

		[Test]
		public void BindsCorrectly()
		{
			this.target.dict.Should().NotBeNull();
			this.target.dict.Count.Should().Be(3);
			this.target.dict["keyA"].Should().Be("valueA");
			this.target.dict["keyB"].Should().Be("valueB");
			this.target.dict["keyC"].Should().Be("valueC");
		}

		class TestResource
		{
			public Dictionary<string,string> dict { get; set; }
		}
	}
}
