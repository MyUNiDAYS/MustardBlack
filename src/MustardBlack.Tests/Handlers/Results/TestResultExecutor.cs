using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;

namespace MustardBlack.Tests.Handlers.Results
{
	public class TestResultExecutor : ResultExecutor<TestResult>
	{
		public override Task Execute(PipelineContext context, TestResult result)
		{
			context.Items["TypeResultExecuted"] = true;

			return Task.CompletedTask;
		}
	}
}