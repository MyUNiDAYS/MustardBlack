using System.IO;
using System.Text;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Json
{
	public class WhenBindingSomeComplexJson : BindingSpecification
	{
		Json target;

		protected override void Given()
		{
			base.Given();

			var json = @"
{
	""Array"": [
		{ 
			""Null"": null,
			""Strings"": [ ""foo1"", ""foo2"" ]
		},
		{ 
			""Null"": null,
			""Strings"": [ ""foo3"", ""foo4"" ]
		}
	],
	""Single"": { 
		""Strings"": [ ""foo5"" ]
	}
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
			this.target.Array.Should().NotBeNull();
			this.target.Array.Length.Should().Be(2);

			this.target.Array[0].Null.Should().BeNull();
			this.target.Array[0].Strings.Length.Should().Be(2);
			this.target.Array[0].Strings[0].Should().Be("foo1");
			this.target.Array[0].Strings[1].Should().Be("foo2");

			this.target.Array[1].Null.Should().BeNull();
			this.target.Array[1].Strings.Length.Should().Be(2);
			this.target.Array[1].Strings[0].Should().Be("foo3");
			this.target.Array[1].Strings[1].Should().Be("foo4");

			this.target.Single.Should().NotBeNull();
			this.target.Single.Null.Should().BeNull();
			this.target.Single.Strings.Length.Should().Be(1);
			this.target.Single.Strings[0].Should().Be("foo5");
		}

		class Json
		{
			public Inner[] Array { get; set; }

			public Inner Single { get; set; }

			public class Inner
			{
				public object Null { get; set; }

				public string[] Strings { get; set; }
			}
		}
	}
}
