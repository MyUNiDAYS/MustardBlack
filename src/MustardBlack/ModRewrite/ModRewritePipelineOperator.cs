using System.Collections.Generic;
using System.Threading.Tasks;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;

namespace MustardBlack.ModRewrite
{
	public sealed class ModRewritePipelineOperator : IPreHandlerExecutionPipelineOperator
	{
		readonly IContainer container;
		IEnumerable<Ruleset> rulesets;

		public ModRewritePipelineOperator(IContainer container, IFileSystem fileSystem)
		{
			this.container = container;
			fileSystem.Read("~/.htaccess", reader => this.rulesets = Parser.Parse(reader.ReadToEnd()));
		}

		public Task<PipelineContinuation> Operate(PipelineContext context)
		{
			var result = Engine.Execute(rulesets, context.Request);
			
			if (result != null)
			{
				context.Result = result;

				var resultExecutorType = typeof(IResultExecutor<>).MakeGenericType(context.Result.GetType());
				var resultExecutor = this.container.Resolve(resultExecutorType) as IResultExecutor;
				resultExecutor?.Execute(context, context.Result);

				return Task.FromResult(PipelineContinuation.End);
			}
			

			return Task.FromResult(PipelineContinuation.Continue);
		}
	}
}
