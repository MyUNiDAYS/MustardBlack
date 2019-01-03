using System;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using NanoIoC;
using Serilog;

namespace MustardBlack.Results
{
	public sealed class ResultExecutorPipelineOperator : IPostResultPipelineOperator
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		readonly IContainer container;

		public ResultExecutorPipelineOperator(IContainer container)
		{
			this.container = container;
		}

		public Task<PipelineContinuation> Operate(PipelineContext context)
		{
			if(context.Result == null)
				return Task.FromResult(PipelineContinuation.Continue);

			var resultExecutorType = this.GetResultExecutorType(context.Result);

			if (resultExecutorType == null)
			{
				log.Error("Cannot locate IResultExecutor for {resultType}", context.Result.GetType());
			}
			else
			{
				var resultExecutor = this.container.Resolve(resultExecutorType) as IResultExecutor;
				resultExecutor.Execute(context, context.Result);
			}
			
			return Task.FromResult(PipelineContinuation.Continue);
		}

		Type GetResultExecutorType(IResult result)
		{
			var resultType = result.GetType();

			while (resultType != null && resultType.IsOrDerivesFrom<IResult>())
			{
				var resultExecutorType = typeof(IResultExecutor<>).MakeGenericType(resultType);
				if (this.container.HasRegistrationsFor(resultExecutorType))
					return resultExecutorType;

				resultType = resultType.BaseType;
			}

			return null;
		}
	}
}
