using MustardBlack.Pipeline;
using MustardBlack.Results;
using MustardBlack.TempData;
using MustardBlack.Tests.Helpers;
using MustardBlack.Tests.Services;

namespace MustardBlack.Tests.Results.JsonResultExecutorSpecs
{
	public class Strings : Specification
	{
		JsonResultExecutor subject;
		JsonResult result;
		PipelineContext pipelineContext;
		TestResponse testResponse;
		string json;
		string entity;

		protected override void Given()
		{
			this.testResponse = new TestResponse();
			this.pipelineContext = new PipelineContext(new TestRequest(), this.testResponse);
			this.subject = new JsonResultExecutor(new NoopTempDataMechanism());
			this.json = "{ \"json\": \"i am\" }";
			this.result = new JsonResult(this.json);
		}

		protected override void When()
		{
			this.subject.Execute(this.pipelineContext, this.result);
		}

		[Then]
		public void ExactJsonShouldBeRenderedToResponse()
		{
			this.entity = this.testResponse.GetEntityString();
			this.entity.ShouldEqual(json);
		}
	}
}