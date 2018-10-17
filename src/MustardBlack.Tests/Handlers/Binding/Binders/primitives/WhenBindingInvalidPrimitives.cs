using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding.Binders.primitives
{
	public class WhenBindingInvalidPrimitives : BindingSpecification
	{
		TestClass target;

		protected override void Given()
		{
			base.Given();

			this.Request.Form.Add("Integer", "1X");
			this.Request.Form.Add("Decimal", "2.Y");
			this.Request.Form.Add("Double", "3.Z");
			this.Request.Form.Add("Char", "ABC");
			this.Request.Form.Add("Byte", "1024");

			this.Request.Url.Returns(new Url("https://www.test.com"));

			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.HttpMethod = HttpMethod.Post;
		}

		protected override void When()
		{
			var binder = BinderCollection.FindBinderFor("outer", typeof(TestClass), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("outer", typeof(TestClass), this.Request, new RouteValues(), false, null);
			this.target = (TestClass)bindingResult.Object;
		}

		[Then]
		public void TheValueShouldBeEmpty()
		{
			this.target.Integer.ShouldEqual(0);
			this.target.Decimal.ShouldEqual(0);
			this.target.Double.ShouldEqual(0);
			this.target.Char.ShouldEqual('\0');
			this.target.Byte.ShouldEqual((byte)0);
		}

		sealed class TestClass
		{
			public int Integer { get; set; }
			public decimal Decimal { get; set; }
			public double Double { get; set; }
			public char Char { get; set; }
			public byte Byte { get; set; }

			public TestClass()
			{
				this.Integer = 10;
				this.Decimal = 10;
				this.Double = 10;
				this.Char = 'X';
				this.Byte = 10;
			}
		}
	}
}
