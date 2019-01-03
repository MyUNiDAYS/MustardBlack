using MustardBlack.Results;
using NanoIoC;

namespace MustardBlack.Tests.Handlers.Results.ExecutorSpecs
{
	public class TypeHeirarchyFallbackSpec : BaseSpecification
	{
		protected override void Given()
		{
			base.Given();
			
			this.container.Register<IResultExecutor<TestResult>, TestResultExecutor>();

			this.context.Result = new DerivedTestResult();
		}
		
		[Then]
		public void ShouldExecuteBaseTypesExecutor()
		{
			this.context.Items["TypeResultExecuted"].ShouldEqual(true);
		}
		
		public class DerivedTestResult : TestResult
		{
		}
	}
}