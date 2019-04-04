using System.Collections.Generic;
using System.Linq;
using MustardBlack.Handlers.Binding;
using MustardBlack.Handlers.Binding.Binders;
using MustardBlack.Hosting;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding.Binders.EnumerableOfStrings
{
	public class EnumerableOfStringsBinderSpecs
	{
		[Then]
		public void ShouldBindNewlineSeparatedStrings()
		{
			var input = "LINE1\nline2\r\nline3";
			var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual("LINE1");
			strings[1].ShouldEqual("line2");
			strings[2].ShouldEqual("line3");
		}

		[Then]
		public void ShouldBindCommaSeparatedStrings()
		{
			var input = "LINE1,line2,line3";
			var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual("LINE1");
			strings[1].ShouldEqual("line2");
			strings[2].ShouldEqual("line3");
		}

		[Then]
		public void ShouldLeaveEmptyLines()
		{
			var input = "LINE1\n\r\nline3";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual("LINE1");
			strings[1].ShouldEqual("");
			strings[2].ShouldEqual("line3");
		}

		[Then]
		public void ShouldLeaveEmptyCommas()
		{
			var input = "LINE1,,line3";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual("LINE1");
			strings[1].ShouldEqual("");
			strings[2].ShouldEqual("line3");
		}

		[Then]
		public void ShouldLeaveNonBreakingWhitespaceWithLines()
		{
			var input = " LINE1 \n  \r\nline3 ";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual(" LINE1 ");
			strings[1].ShouldEqual("  ");
			strings[2].ShouldEqual("line3 ");
		}

		[Then]
		public void ShouldLeaveNonBreakingWhitespaceWithCommas()
		{
			var input = " LINE1 ,  ,line3 ";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual(" LINE1 ");
			strings[1].ShouldEqual("  ");
			strings[2].ShouldEqual("line3 ");
		}

		[Then]
		public void MostPresentCommaOrLineWins()
		{
			var input = "LINE1\nLINE2,line3,LINE4";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual("LINE1\nLINE2");
			strings[1].ShouldEqual("line3");
			strings[2].ShouldEqual("LINE4");
		}

		[Then]
		public void MostPresentCommaOrLineWins2()
		{
			var input = "LINE1,LINE2\nline3\nLINE4";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual("LINE1,LINE2");
			strings[1].ShouldEqual("line3");
			strings[2].ShouldEqual("LINE4");
		}
	}
}
