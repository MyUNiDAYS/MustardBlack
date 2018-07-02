namespace MustardBlack.Pipeline
{
	public interface IAttributePipelineOperator<TAttribute> : IAttributePipelineOperator
	{
	}

	public interface IAttributePipelineOperator : IPreHandlerExecutionPipelineOperator
	{
		int Order { get; }
		object Attribute { set; }
	}
}
