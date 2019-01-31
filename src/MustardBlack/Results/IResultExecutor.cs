using System.Threading.Tasks;
using MustardBlack.Pipeline;

namespace MustardBlack.Results
{
	public interface IResultExecutor
	{
		Task Execute(PipelineContext context, object result);
	}
}