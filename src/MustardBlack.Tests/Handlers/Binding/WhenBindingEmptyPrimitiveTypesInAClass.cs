using System.Collections.Specialized;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding
{
    public class WhenBindingEmptyPrimitiveTypesInAClass : BindingSpecification
    {
        private NameValueCollection post;
        private PrimitivesClass target;

        protected override void Given()
        {
            base.Given();

            this.post = new NameValueCollection
			       	{
			       		{"sbyteus", ""},
			       		{"byteus", ""},
			       		{"shortus", ""},
			       		{"ushortus", ""},
			       		{"intus", ""},
			       		{"uintus", ""},
			       		{"longus", ""},
			       		{"ulongus", ""},
			       		{"charus", ""},
			       		{"floatus", ""},
			       		{"doubleus", ""},
			       		{"boolus", ""},
			       		{"decimalus", ""},
			       		{"stringus", ""}
			       	};

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));

        }

        protected override void When()
        {
            var binder = BinderCollection.FindBinderFor(null, typeof(PrimitivesClass), this.Request, new RouteValues(), null);
            var bindingResult = binder.Bind("parameter", typeof(PrimitivesClass), this.Request, new RouteValues(), false, null);
            this.target = bindingResult.Object as PrimitivesClass;
        }

        [Test]
        public void TheIntShouldBeSetToItsDefaultValue()
        {
	        this.target.sbyteus.Should().Be(default(sbyte));
	        this.target.byteus.Should().Be(default(byte));
	        this.target.shortus.Should().Be(default(short));
	        this.target.intus.Should().Be(default(int));
	        this.target.uintus.Should().Be(default(uint));
	        this.target.longus.Should().Be(default(long));
	        this.target.ulongus.Should().Be(default(ulong));
	        this.target.charus.Should().Be(default(char));
	        this.target.floatus.Should().Be(default(float));
	        this.target.doubleus.Should().Be(default(double ));
	        this.target.boolus.Should().Be(default(bool));
	        this.target.decimalus.Should().Be(default(decimal));
	        this.target.stringus.Should().Be("");
        }
    }
}
