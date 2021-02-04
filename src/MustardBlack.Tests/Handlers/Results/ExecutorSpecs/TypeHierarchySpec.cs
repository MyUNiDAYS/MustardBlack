using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;

namespace MustardBlack.Tests.Handlers.Results.ExecutorSpecs
{
	public class TypeHierarchySpec : BaseSpecification
	{
		protected override void Given()
		{
			base.Given();
			
			this.container.Register<IResultExecutor<TestResult>, TestResultExecutor>();
			this.container.Register<IResultExecutor<DerivedTestResult>, DerivedTestResultExecutor>();

			this.context.Result = new DerivedTestResult();
		}
		
		[Then]
		public void ShouldExecuteDerivedTypesExecutor()
		{
			this.context.Items.ContainsKey("TypeResultExecuted").ShouldBeFalse();
			this.context.Items["DerivedExecuted"].ShouldEqual(true);
		}
		
		public class DerivedTestResult : TestResult
		{
		}

		public class DerivedTestResultExecutor : ResultExecutor<DerivedTestResult>
		{
			public override async Task Execute(PipelineContext context, DerivedTestResult result)
			{
				context.Items["DerivedExecuted"] = true;
			}
		}
	}
}