using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	public interface IResultExecutor<in TResult> : IResultExecutor where TResult : IResult
	{
		void Execute(PipelineContext context, TResult result);
	}
}