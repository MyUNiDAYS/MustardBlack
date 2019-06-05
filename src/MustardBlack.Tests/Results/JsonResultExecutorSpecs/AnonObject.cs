using MustardBlack.Pipeline;
using MustardBlack.Results;
using MustardBlack.Tests.Helpers;
using MustardBlack.Tests.Services;

namespace MustardBlack.Tests.Results.JsonResultExecutorSpecs
{
	public class AnonObject : Specification
	{
		JsonResultExecutor subject;
		JsonResult result;
		PipelineContext pipelineContext;
		TestResponse testResponse;
		string entity;

		protected override void Given()
		{
			this.testResponse = new TestResponse();
			this.pipelineContext = new PipelineContext(new TestRequest(), this.testResponse);
			this.subject = new JsonResultExecutor(new NoopTempDataMechanism());
			this.result = new JsonResult(new { foo = "bar" });
		}

		protected override void When()
		{
			this.subject.Execute(this.pipelineContext, this.result);
		}

		[Then]
		public void ExactJsonShouldBeRenderedToResponse()
		{
			this.entity = this.testResponse.GetEntityString();
			this.entity.ShouldEqual("{\"foo\":\"bar\"}");
		}
	}
}