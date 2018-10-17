using System.Collections.Generic;
using System.Linq;
using MustardBlack.Handlers.Binding;
using MustardBlack.Handlers.Binding.Binders;
using MustardBlack.Hosting;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding.Binders
{
	public class EnumerableOfStringsBinderSpecs
	{
		[Then]
		public void ShouldBindnewlineSeparatedStrings()
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
		public void ShouldLeaveNonBreakingWhitespace()
		{
			var input = " LINE1 \n  \r\nline3 ";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.ShouldEqual(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].ShouldEqual(" LINE1 ");
			strings[1].ShouldEqual("  ");
			strings[2].ShouldEqual("line3 ");
		}
	}
}
