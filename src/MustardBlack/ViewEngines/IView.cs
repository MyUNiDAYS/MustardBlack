using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;

namespace MustardBlack.ViewEngines
{
	public interface IView
	{
		IContainer Container { get; set; }
		ViewResult ViewResult { get; set; }
		PipelineContext PipelineContext { get; set; }
	}
}