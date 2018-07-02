using System;
using System.Text;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class RazorViewRenderer : ViewRendererBase
	{
		readonly IViewResolver viewResolver;

		public RazorViewRenderer(IViewResolver viewResolver)
		{
			this.viewResolver = viewResolver;
		}

		public override bool CanRender(Type viewType)
		{
			return viewType.IsOrDerivesFrom(typeof(RazorViewPage));
		}

		public override StringBuilder Render(ViewResult viewResult, ViewRenderingContext context)
		{
			var view = this.GetViewInstance(viewResult, context);

			view.ExecuteView(null, null);

			var body = view.Body;
			var sectionContents = view.SectionContents;

			// null means no override, string.Empty means ensure there is no layout
			if (viewResult.MasterPageOverride != null)
				view.Layout = viewResult.MasterPageOverride;

			var root = string.IsNullOrWhiteSpace(view.Layout);

			// render the initial view, and move "up" through its parent layouts
			while (!root)
			{
				Type masterType;
				try
				{
					masterType = this.viewResolver.Resolve(view.Layout);
				}
				catch (Exception ex)
				{
					throw new Exception($"Unable to resolve layout for view: {view}", ex);
				}

				var masterResult = new ViewResult(masterType, viewResult.AreaName, viewResult.ViewData, viewResult.StatusCode);
				view = this.GetViewInstance(masterResult, context);

				view.ExecuteView(body, sectionContents);

				body = view.Body;
				sectionContents = view.SectionContents;

				root = !view.HasLayout;
			}

			return new StringBuilder(body);
		}

		RazorViewPage GetViewInstance(ViewResult viewResult, ViewRenderingContext context)
		{
			var view = Activator.CreateInstance(viewResult.ViewType) as RazorViewPage;

			this.HydrateView(viewResult, view, context);

			return view;
		}
	}
}
