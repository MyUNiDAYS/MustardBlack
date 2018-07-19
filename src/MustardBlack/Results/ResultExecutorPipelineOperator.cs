using System.Threading.Tasks;
using MustardBlack.Pipeline;
using NanoIoC;

namespace MustardBlack.Results
{
	public sealed class ResultExecutorPipelineOperator : IPostResultPipelineOperator
	{
		readonly IContainer container;

		public ResultExecutorPipelineOperator(IContainer container)
		{
			this.container = container;
		}

		public Task<PipelineContinuation> Operate(PipelineContext context)
		{
			if(context.Result == null)
				return Task.FromResult(PipelineContinuation.Continue);

			var resultExecutorType = typeof(IResultExecutor<>).MakeGenericType(context.Result.GetType());
			var resultExecutor = this.container.Resolve(resultExecutorType) as IResultExecutor;
			resultExecutor?.Execute(context, context.Result);

			return Task.FromResult(PipelineContinuation.Continue);
		}
	}
}
