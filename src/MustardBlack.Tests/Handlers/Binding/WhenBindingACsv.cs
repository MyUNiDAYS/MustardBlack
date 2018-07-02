using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class WhenBindingACsv : BindingSpecification
	{
		NameValueCollection post;
		TestClass target;

		protected override void Given()
		{
			base.Given();

			this.post = new NameValueCollection
			            	{
			            		{"table", @"1,2
1,2
1,4
1,5

13,4

" }

			            	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.dom.com", 80, "/path", null));
		}

		protected override void When()
		{
            var binder = BinderCollection.FindBinderFor(null, typeof(TestClass), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(TestClass), this.Request, new RouteValues(), false, null);
			this.target = bindingResult.Object as TestClass;
		}

		[Test]
		public void TheResultsShouldBeBoundCorrectly()
		{
			this.target.Table.ToList()[0].ToList()[0].Should().Be(1);
			this.target.Table.ToList()[0].ToList()[1].Should().Be(2);
			this.target.Table.ToList()[2].ToList()[0].Should().Be(1);
			this.target.Table.ToList()[2].ToList()[1].Should().Be(4);
		}

		public class TestClass
		{
			public IEnumerable<IEnumerable<int>> Table { get; set; }
		}
	}
}