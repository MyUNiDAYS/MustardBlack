using System.Collections.Generic;
using MustardBlack.Pipeline;

namespace MustardBlack.TempData
{
	public interface ITempDataMechanism
	{
		void SetTempData(PipelineContext context, IDictionary<string, object> tempData);
	}
}