namespace MustardBlack.Pipeline
{
	public interface IAttributePipelineOperator<TAttribute> : IAttributePipelineOperator
	{
	}

	public interface IAttributePipelineOperator : IPreResultPipelineOperator
	{
		int Order { get; }
		object Attribute { set; }
	}
}
