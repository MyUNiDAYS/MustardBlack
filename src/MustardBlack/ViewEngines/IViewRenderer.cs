using System;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	/// <summary>
	/// Renders a view
	/// </summary>
	public interface IViewRenderer
	{
		bool CanRender(Type viewType);
		Task Render(ViewResult viewResult, PipelineContext context, ViewRenderingContext viewRenderingContext);
	}
}