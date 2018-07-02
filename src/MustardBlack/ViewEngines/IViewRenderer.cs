using System;
using System.Text;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	/// <summary>
	/// Renders a view
	/// </summary>
	public interface IViewRenderer
	{
		bool CanRender(Type viewType);
		StringBuilder Render(ViewResult viewResult, ViewRenderingContext context);
	}
}