using System.Threading.Tasks;
using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	public interface IResultExecutor<in TResult> : IResultExecutor where TResult : IResult
	{
		Task Execute(PipelineContext context, TResult result);
	}
}