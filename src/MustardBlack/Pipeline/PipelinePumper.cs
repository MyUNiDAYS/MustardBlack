using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;

namespace MustardBlack.Pipeline
{
	/// <summary>
	/// Executes IPipelineOperators
	/// </summary>
	public static class PipelinePumper
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Executes the given handler with the given PipelineContext
		/// </summary>
		/// <param name="pipelineContext"></param>
		/// <param name="pipelineOperators"></param>
		/// <returns></returns>
		public static async Task<PipelineContinuation> Pump(PipelineContext pipelineContext, IEnumerable<IPipelineOperator> pipelineOperators)
		{
			var continuation = PipelineContinuation.Continue;

			foreach (var pipelineOperator in pipelineOperators)
			{
				if (continuation == PipelineContinuation.SkipToPostHandler && pipelineOperator is IPreResultPipelineOperator)
				{
					log.Debug("Skipping {operator}, searching for next IPostHandlerExecututionPipelineOperator", pipelineOperator.GetType());
					continue;
				}

				if (continuation == PipelineContinuation.End)
				{
					log.Debug("Ending Pipeline pumping", pipelineOperator.GetType());
					break;
				}
				
				log.Debug("Running {operator}", pipelineOperator.GetType());
				continuation = await pipelineOperator.Operate(pipelineContext);
				
				log.Debug("{operator} returned {continuation}", pipelineOperator.GetType(), continuation);
			}

			return continuation;
		}
	}
}