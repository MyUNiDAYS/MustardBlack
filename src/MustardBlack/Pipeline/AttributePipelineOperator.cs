using System;
using System.Threading.Tasks;

namespace MustardBlack.Pipeline
{
	public abstract class AttributePipelineOperator<TAttribute> : IAttributePipelineOperator<TAttribute> where TAttribute : Attribute
	{
		public abstract int Order { get; }
		protected TAttribute Attribute { get; private set; }

		object IAttributePipelineOperator.Attribute
		{
			set => this.Attribute = (TAttribute)value;
		}

		public abstract Task<PipelineContinuation> Operate(PipelineContext context);
	}
}
