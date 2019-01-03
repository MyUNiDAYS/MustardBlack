using MustardBlack.Pipeline;
using MustardBlack.Results;
using MustardBlack.Tests.Handlers.Results.ExecutorSpecs;

namespace MustardBlack.Tests.Handlers.Results
{
	public class TestResultExecutor : ResultExecutor<TestResult>
	{
		public override void Execute(PipelineContext context, TestResult result)
		{
			context.Items["TypeResultExecuted"] = true;
		}
	}
}