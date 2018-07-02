using MustardBlack.ModRewrite;
using MustardBlack.Results;
using MustardBlack.Tests.Helpers;

namespace MustardBlack.Tests.ModRewrite
{
	public abstract class ModRewriteSpecification : Specification
	{
		protected abstract string Rules { get; }
		protected abstract string RequestUrl { get; }
		protected TestRequest request;
		protected IResult handledResponse;

		protected override void Given()
		{
			this.request = new TestRequest();
			this.request.Url = new Url(this.RequestUrl);
		}

		protected override void When()
		{
			var rules = Parser.Parse(this.Rules);
			this.handledResponse = Engine.Execute(rules, this.request);
		}
		
	}
}
