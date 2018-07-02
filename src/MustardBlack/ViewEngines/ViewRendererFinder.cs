using System;
using System.Collections.Generic;
using System.Linq;

namespace MustardBlack.ViewEngines
{
	sealed class ViewRendererFinder : IViewRendererFinder
	{
		readonly IEnumerable<IViewRenderer> viewRenderers;

		public ViewRendererFinder(IEnumerable<IViewRenderer> viewRenderers)
		{
			this.viewRenderers = viewRenderers;
		}

		public IViewRenderer FindViewRenderer(Type viewType)
		{
			return this.viewRenderers.FirstOrDefault(r => r.CanRender(viewType));
		}
	}
}