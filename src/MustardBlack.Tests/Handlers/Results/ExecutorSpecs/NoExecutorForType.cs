namespace MustardBlack.Tests.Handlers.Results.ExecutorSpecs
{
	public class NoExecutorForType : BaseSpecification
	{
		protected override void Given()
		{
			base.Given();

			this.context.Result = new TestResult();
		}
		
		[Then]
		public void ShouldExecuteBaseTypesExecutor()
		{
			this.context.Items.ContainsKey("TypeResultExecuted").ShouldBeFalse();
		}
	}
}