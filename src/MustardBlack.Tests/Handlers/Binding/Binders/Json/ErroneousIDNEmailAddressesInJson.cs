using System.IO;
using System.Linq;
using System.Text;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Json
{
	public class ErroneousIDNEmailAddressesInJson : BindingSpecification
	{
		Json target;
		BindingResult bindingResult;

		protected override void Given()
		{
			base.Given();

			var json = @"
{
	""Email"": ""nathan@学生..优惠.com"",
	""OtherEmail"": null,
	""SomethingElse"": """",
	""SomethingElseElse"": """",
	""deepThing"": {
		""EmailAddress"": ""nath?an@学生优惠..com""
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
			this.bindingResult = binder.Bind("json", typeof(Json), this.Request, new RouteValues(), false, null);
			this.target = (Json)bindingResult.Object;
		}

		[Test]
		public void ShouldNotDePunycodeEmailAddressesAndHaveErrors()
		{
			this.target.Email.Should().Be("nathan@学生..优惠.com");
			this.target.DeepThing.EmailAddress.Should().Be("nath?an@学生优惠..com");
			var bindingErrors = this.bindingResult.BindingErrors.ToArray();
			bindingErrors.Length.Should().Be(2);

		}

		[Test]
		public void NonEmailPropertyShouldWork()
		{
			this.target.SomethingElse.Should().Be("");
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