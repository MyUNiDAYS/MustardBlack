using System;
using System.Reflection;

using MuonLab.Validation;
using MustardBlack.Handlers;
using MustardBlack.Handlers.Binding;
using MustardBlack.Results;
using MustardBlack.Routing;
using MustardBlack.Tests.Helpers;
using NanoIoC;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding.RequestBinderSpecs
{
	public class MixedJsonAndRouteValues : Specification
	{
		RequestBinder subject;
		TestHandler handler;
		MethodInfo methodInfo;
		TestRequest request;
		private RouteValues routeValues;
		private object[] parameters;

		protected override void Given()
		{
			var container = new Container();
			subject = new RequestBinder(container, Substitute.For<IErrorMessageResolver>(), new ViolationPropertyNameResolver());
			this.handler = new TestHandler();
			this.methodInfo = this.handler.GetType().GetMethod("Get");
			this.request = new TestRequest();
			this.request.SetRequestBody("{\"Text\":\"some text\",\"Number\":3}");
			this.request.ContentType = "application/json; charset=utf-8";
			this.request.HttpMethod = HttpMethod.Post;
			this.routeValues = new RouteValues
			{
				{"routeParam", "3A814A58-C5B2-408D-BDA8-B80DDC725735"}
			};

			BinderCollection.Initialize(container);
		}

		protected override void When()
		{
			parameters = this.subject.GetAndValidateParameters(this.handler, this.methodInfo, this.request, this.routeValues);

		}

		[Then]
		public void ShouldBindProperly()
		{
			var testResource = parameters[0] as TestResource;
			testResource.Text.ShouldEqual("some text");
			testResource.Number.ShouldEqual(3);

			parameters[1].ShouldEqual(Guid.Parse("3A814A58-C5B2-408D-BDA8-B80DDC725735"));
		}

		sealed class TestHandler : Handler
		{
			public IResult Get(TestResource resource, Guid routeParam)
			{
				return null;
			}
		}

		sealed class TestResource
		{
			public string Text { get; set; }
			public int Number { get; set; }
		}
	}
}