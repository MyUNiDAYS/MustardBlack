using System;

namespace MustardBlack.ViewEngines
{
	public interface IViewRendererFinder
	{
		IViewRenderer FindViewRenderer(Type viewType);
	}
}