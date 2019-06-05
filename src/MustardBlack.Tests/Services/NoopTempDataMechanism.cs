using System.Collections.Generic;
using MustardBlack.Pipeline;
using MustardBlack.TempData;

namespace MustardBlack.Tests.Services
{
	public class NoopTempDataMechanism : ITempDataMechanism
	{
		public void SetTempData(PipelineContext context, IDictionary<string, object> tempData)
		{
			
		}
	}
}