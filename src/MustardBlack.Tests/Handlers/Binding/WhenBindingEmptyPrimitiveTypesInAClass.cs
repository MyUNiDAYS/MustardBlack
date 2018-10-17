using System.Collections.Specialized;

using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

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

        [Then]
        public void TheIntShouldBeSetToItsDefaultValue()
        {
	        this.target.sbyteus.ShouldEqual(default(sbyte));
	        this.target.byteus.ShouldEqual(default(byte));
	        this.target.shortus.ShouldEqual(default(short));
	        this.target.intus.ShouldEqual(default(int));
	        this.target.uintus.ShouldEqual(default(uint));
	        this.target.longus.ShouldEqual(default(long));
	        this.target.ulongus.ShouldEqual(default(ulong));
	        this.target.charus.ShouldEqual(default(char));
	        this.target.floatus.ShouldEqual(default(float));
	        this.target.doubleus.ShouldEqual(default(double ));
	        this.target.boolus.ShouldEqual(default(bool));
	        this.target.decimalus.ShouldEqual(default(decimal));
	        this.target.stringus.ShouldEqual("");
        }
    }
}
