using System.Threading.Tasks;

namespace MustardBlack.Pipeline
{
	/// <summary>
	/// Defines an operation that runs as part of the pipeline
	/// Do not implement directly, implement either IPreHandlerExecututionPipelineOperator or IPostHandlerExecutionPipelineOperator
	/// </summary>
	public interface IPipelineOperator
	{
		/// <summary>
		/// Performs the operation against the pipeline
		/// </summary>
		/// <param name="context"></param>
		Task<PipelineContinuation> Operate(PipelineContext context);
	}
}