using System.IO;
using System.Text;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Json
{
	public class WhenBindingSomeJson : BindingSpecification
	{
		Json target;

		protected override void Given()
		{
			base.Given();

			var json = @"
{
	""thing"": ""foo""
}
";
			var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
			memoryStream.Position = 0;

			this.Request.BufferlessInputStream.Returns(memoryStream);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/json");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));

		}

		protected override void When()
		{
			var binder = BinderCollection.FindBinderFor("json", typeof(Json), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("json", typeof(Json), this.Request, new RouteValues(), false, null);
			this.target = (Json)bindingResult.Object;
		}

		[Test]
		public void ItShouldBind()
		{
			this.target.thing.Should().Be("foo");
		}

		class Json
		{
			public string thing { get; set; }
		}
	}
}