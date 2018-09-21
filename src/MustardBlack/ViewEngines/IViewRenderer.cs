using System;
using System.Text;
using System.Threading.Tasks;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	/// <summary>
	/// Renders a view
	/// </summary>
	public interface IViewRenderer
	{
		bool CanRender(Type viewType);
		Task<StringBuilder> Render(ViewResult viewResult, ViewRenderingContext context);
	}
}