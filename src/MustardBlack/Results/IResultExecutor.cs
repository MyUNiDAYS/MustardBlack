using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	public interface IResultExecutor
	{
		void Execute(PipelineContext context, object result);
	}
}