using MustardBlack.Pipeline;
using MustardBlack.Results;
using MustardBlack.Tests.Helpers;
using NanoIoC;

namespace MustardBlack.Tests.Handlers.Results.ExecutorSpecs
{
	public abstract class BaseSpecification : Specification
	{
		ResultExecutorPipelineOperator subject;
		protected PipelineContext context;
		protected Container container;

		protected override void Given()
		{
			this.container = new Container();

			this.subject = new ResultExecutorPipelineOperator(this.container);

			this.context = new PipelineContext(new TestRequest(), new TestResponse());
		}

		protected override void When()
		{
			this.subject.Operate(this.context);
		}
	}
}