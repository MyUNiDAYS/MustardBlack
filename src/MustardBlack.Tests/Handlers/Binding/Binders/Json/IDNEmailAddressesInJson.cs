using System.IO;
using System.Text;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Json
{
	public class IDNEmailAddressesInJson : BindingSpecification
	{
		Json target;

		protected override void Given()
		{
			base.Given();

			var json = @"
{
	""Email"": ""nathan@学生优惠.com"",
	""OtherEmail"": null,
	""SomethingElse"": ""nathan@学生优惠.com"",
	""SomethingElseElse"": """",
	""deepThing"": {
		""EmailAddress"": ""nathan@学生优惠.com""
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
		public void ShouldDePunycodeEmailAddresses()
		{
			this.target.Email.Should().Be("nathan@xn--4oqu31ahiekqy.com");
			this.target.DeepThing.EmailAddress.Should().Be("nathan@xn--4oqu31ahiekqy.com");
		}

		[Test]
		public void NonEmailPropertyShouldWork()
		{
			this.target.SomethingElse.Should().Be("nathan@学生优惠.com");
			this.target.SomethingElseElse.Should().Be("");
		}

		[Test]
		public void EmptyEmailAddressesShouldWork()
		{
			this.target.OtherEmail.Should().BeNull();
			this.target.AnotherEmail.Should().BeNull();
		}

		public class Json
		{
			public string Email { get; set; }
			public string OtherEmail { get; set; }
			public string AnotherEmail { get; set; }
			public string SomethingElse { get; set; }
			public string SomethingElseElse { get; set; }
			public Deep DeepThing { get; set; }

			public class Deep
			{
				public string EmailAddress { get; set; }
			}
		}
	}
}