using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MustardBlack.Handlers.Binding;
using MustardBlack.Handlers.Binding.Binders;
using MustardBlack.Hosting;
using MustardBlack.Routing;
using NSubstitute;
using NUnit.Framework;

namespace MustardBlack.Tests.Handlers.Binding.Binders
{
	[TestFixture]
	public class EnumerableOfStringsBinderSpecs
	{
		[Then]
		public void ShouldBindnewlineSeparatedStrings()
		{
			var input = "LINE1\nline2\r\nline3";
			var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.Should().Be(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].Should().Be("LINE1");
			strings[1].Should().Be("line2");
			strings[2].Should().Be("line3");
		}

		[Then]
		public void ShouldLeaveEmptyLines()
		{
			var input = "LINE1\n\r\nline3";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.Should().Be(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].Should().Be("LINE1");
			strings[1].Should().Be("");
			strings[2].Should().Be("line3");
		}


		[Then]
		public void ShouldLeaveNonBreakingWhitespace()
		{
			var input = " LINE1 \n  \r\nline3 ";
            var bindingResult = new EnumerableOfStringsBinder().Bind(typeof(IEnumerable<string>), null, input, Substitute.For<IRequest>(), new RouteValues(), null);

			bindingResult.Result.Should().Be(BindingResult.ResultType.Success);
			var strings = (bindingResult.Object as IEnumerable<string>).ToArray();

			strings[0].Should().Be(" LINE1 ");
			strings[1].Should().Be("  ");
			strings[2].Should().Be("line3 ");
		}
	}
}
