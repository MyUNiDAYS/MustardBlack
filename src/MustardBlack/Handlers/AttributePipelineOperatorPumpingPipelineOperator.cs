using System.Linq;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using NanoIoC;

namespace MustardBlack.Handlers
{
	public sealed class AttributePipelineOperatorPumpingPipelineOperator : IPreResultPipelineOperator
	{
		readonly IContainer container;

		public AttributePipelineOperatorPumpingPipelineOperator(IContainer container)
		{
			this.container = container;
		}

		public Task<PipelineContinuation> Operate(PipelineContext context)
		{
			var handlerAction = context.RouteData()?.HandlerAction;

			if (handlerAction == null)
				return Task.FromResult(PipelineContinuation.Continue);

			var operators = handlerAction.Operators.SelectMany(o =>
				{
					var attributeOperators = this.container.ResolveAll(o.OperatorType).Cast<IAttributePipelineOperator>();

					foreach (var attributeOperator in attributeOperators)
						attributeOperator.Attribute = o.Attribute;

					return attributeOperators;
				})
				.OrderBy(a => a.Order)
				.ToArray();

			return PipelinePumper.Pump(context, operators);
		}
	}
}
