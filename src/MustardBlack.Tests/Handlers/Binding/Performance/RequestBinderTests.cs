using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using MustardBlack.Handlers;
using MustardBlack.Handlers.Binding;
using MustardBlack.Results;
using MustardBlack.Routing;
using NanoIoC;
using Xunit;

namespace MustardBlack.Tests.Handlers.Binding.Performance
{
	public class RequestBinderTests : AutoMocker
	{
		[ThenExplicit]
		public void IntTest()
		{
			this.DoBindToForm("Int", CreateFormValues("7"), 10000);
		}

		[ThenExplicit]
		public void BoolTest()
		{
			this.DoBindToForm("Bool", CreateFormValues("true"), 10000);
		}

		[ThenExplicit]
		public void GuidTest()
		{
			this.DoBindToForm("Guid", CreateFormValues(Guid.NewGuid().ToString()), 10000);
		}

		[ThenExplicit]
		public void StringTest()
		{
			this.DoBindToForm("String", CreateFormValues("string-value"), 10000);
		}

		[ThenExplicit]
		public void EnumTest()
		{
			this.DoBindToForm("Enum", CreateFormValues(((int)DateTimeKind.Utc).ToString()), 10000);
		}

		[TheoryExplicit]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(100)]
		[InlineData(1000)]
		public void EnumerableTest(int count)
		{
			this.DoBindToForm("Enumerable", CreateFormValues(string.Join(",", Enumerable.Range(0, count))), 10000);
		}

		static NameValueCollection CreateFormValues(string value)
		{
			return new NameValueCollection
			{
				["parameter"] = value
			};
		}

		[TheoryExplicit]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(100)]
		[InlineData(1000)]
		public void ListTest(int count)
		{
			var formValues = new NameValueCollection();
			foreach (var n in Enumerable.Range(0, count))
				formValues[$"parameter[{n}]"] = n.ToString();

			this.DoBindToForm("List", formValues, 10000);
		}

		[ThenExplicit]
		public void ComplexTypeTest()
		{
			var formValues = new NameValueCollection();
			SetFormValues(formValues, "parameter");
			this.DoBindToForm("ComplexType", formValues, 10000);
		}

		[ThenExplicit]
		public void ResourceTest()
		{
			var formValues = new NameValueCollection();
			SetFormValues(formValues, "");
			foreach (var n in Enumerable.Range(0, 20))
				formValues[$"IntList[{n}]"] = n.ToString();
			foreach (var n in Enumerable.Range(0, 20))
				formValues[$"StringList[{n}]"] = $"listitem{n}";
			foreach (var n in Enumerable.Range(0, 20))
				SetFormValues(formValues, $"ComplexList[{n}].");

			this.DoBindToForm("Resource", formValues, 10000);
		}

		static void SetFormValues(NameValueCollection formValues, string name)
		{
			formValues[$"{name}Id"] = Guid.NewGuid().ToString();
			formValues[$"{name}Name"] = $"{name}key";
			formValues[$"{name}Number"] = "123";
		}

		[ThenExplicit]
		public void ComplexResourceTest()
		{
			var formValues = new NameValueCollection();
			SetFormValues(formValues, "");
			foreach (var n in Enumerable.Range(0, 20))
				formValues[$"IntList[{n}]"] = n.ToString();
			foreach (var n in Enumerable.Range(0, 20))
				formValues[$"StringList[{n}]"] = $"listitem{n}";
			foreach (var n in Enumerable.Range(0, 20))
				SetFormValues(formValues, $"ComplexList[{n}].");
			foreach (var n in Enumerable.Range(0, 10))
			{
				SetFormValues(formValues, $"ResourceList[{n}].");
				foreach (var m in Enumerable.Range(0, 100))
					formValues[$"ResourceList[{n}].IntList[{m}]"] = m.ToString();
				foreach (var m in Enumerable.Range(0, 100))
					formValues[$"ResourceList[{n}].StringList[{m}]"] = $"listitem{n}{m}";
				foreach (var m in Enumerable.Range(0, 100))
					SetFormValues(formValues, $"ResourceList[{n}].ComplexList[{m}].");

			}

			this.DoBindToForm("ComplexResource", formValues, 1);
		}

		void DoBindToForm(string method, NameValueCollection formValues, int bindIterations)
		{
			BinderCollection.Initialize(Container.Global);

			var parameterInfos = typeof(TestHandler).GetMethod(method)?.GetParameters();

			var request = new TestRequest
			{
				HttpMethod = HttpMethod.Post,
				ContentType = "multipart/form-data",
				Form = formValues
			};

			var requestBinder = this.Subject<RequestBinder>();

			var stopwatch = Stopwatch.StartNew();
			for (var i = 0; i < bindIterations; i++)
			{
				requestBinder.Bind(null, parameterInfos?.Single(), request, new RouteValues());
			}
			Console.WriteLine(stopwatch.Elapsed);
		}

		class TestHandler : Handler
		{
			public IResult Int(int parameter)
			{
				return new EmptyResult();
			}

			public IResult Bool(bool parameter)
			{
				return new EmptyResult();
			}

			public IResult Guid(Guid parameter)
			{
				return new EmptyResult();
			}

			public IResult String(string parameter)
			{
				return new EmptyResult();
			}

			public IResult Enum(DateTimeKind parameter)
			{
				return new EmptyResult();
			}

			public IResult Enumerable(IEnumerable<int> parameter)
			{
				return new EmptyResult();
			}

			public IResult List(IList<int> parameter)
			{
				return new EmptyResult();
			}

			public IResult ComplexType(ComplexType parameter)
			{
				return new EmptyResult();
			}

			public IResult Resource(Resource parameter)
			{
				return new EmptyResult();
			}

			public IResult ComplexResource(ComplexResource parameter)
			{
				return new EmptyResult();
			}
		}
	}

	public class ComplexType
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
	}

	public class Resource
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
		public IList<int> IntList { get; set; }
		public IList<string> StringList { get; set; }
		public IList<ComplexType> ComplexList { get; set; }
	}

	public class ComplexResource
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
		public IList<int> IntList { get; set; }
		public IList<string> StringList { get; set; }
		public IList<ComplexType> ComplexList { get; set; }
		public IList<Resource> ResourceList { get; set; }
	}
}
