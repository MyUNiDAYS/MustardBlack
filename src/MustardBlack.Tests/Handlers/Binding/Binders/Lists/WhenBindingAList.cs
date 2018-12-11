using System.Collections.Generic;
using System.Collections.Specialized;
using MustardBlack.Handlers.Binding;
using MustardBlack.Routing;
using NSubstitute;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding.Binders.Lists
{
    public class WhenBindingAList : BindingSpecification
    {
        NameValueCollection post;
	    TestResource target;

        protected override void Given()
        {
            base.Given();

            this.post = new NameValueCollection
	                        {
		                        {"Listy[0]", "string 0"},
		                        {"Listy[1]", "string 1"},
		                        {"Listy[3]", "string 3"}
	                        };

			this.Request.Form.Returns(this.post);
			this.Request.HttpMethod = HttpMethod.Post;
			this.Request.ContentType.Returns("application/x-www-form-urlencoded");
			this.Request.Url.Returns(new Url("http", "www.mydomain.com", 80, "/path", null));

        }

        protected override void When()
        {
			var binder = BinderCollection.FindBinderFor("testy", typeof(TestResource), this.Request, new RouteValues(), null);
			var bindingResult = binder.Bind("testy", typeof(TestResource), this.Request, new RouteValues(), false, null);
            this.target = (TestResource)bindingResult.Object;
        }

        [Then]
        public void TheArrayShouldNotBeTouched()
        {
            this.target.Arrayy.ShouldBeNull();
        }

        [Then]
        public void TheListShouldBeCorrectlyPopulated()
        {
	        this.target.Listy[0].ShouldEqual("string 0");
	        this.target.Listy[1].ShouldEqual("string 1");
	        this.target.Listy[2].ShouldBeNull();
	        this.target.Listy[3].ShouldEqual("string 3");
			this.target.Listy.Count.ShouldEqual(4);
        }

	    public class TestResource
	    {
		    public IList<string> Listy { get; set; }
		    public string[] Arrayy { get; set; }

		    public TestResource()
		    {
			    this.Listy = new string[0];
		    }
	    }
    }
}
