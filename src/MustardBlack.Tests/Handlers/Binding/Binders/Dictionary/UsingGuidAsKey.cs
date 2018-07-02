using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Dictionary
{
	public class UsingGuidAsKey : BindingSpecification
	{
		NameValueCollection post;
		TestResource target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
					   {
						   {"dict[d01c7797-fa85-4d78-98ea-8127467dceba]", "guidA"},
						   {"dict[9d2a40d2-8349-4e65-8cac-92c494c61722]", "guidB"},
						   {"dict[92bb53d7-279f-44d9-949f-688bfbd46265]", "guidC"},
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
			this.target.dict[new Guid("d01c7797-fa85-4d78-98ea-8127467dceba")].Should().Be("guidA");
			this.target.dict[new Guid("9d2a40d2-8349-4e65-8cac-92c494c61722")].Should().Be("guidB");
			this.target.dict[new Guid("92bb53d7-279f-44d9-949f-688bfbd46265")].Should().Be("guidC");
		}

		class TestResource
		{
			public Dictionary<Guid,string> dict { get; set; }
		}
	}
}
