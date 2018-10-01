using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	public abstract class ViewRendererBase : IViewRenderer
	{
	    protected readonly HtmlEncoder htmlEncoder;

	    protected ViewRendererBase(HtmlEncoder htmlEncoder)
	    {
	        this.htmlEncoder = htmlEncoder;
	    }

	    public abstract bool CanRender(Type viewType);
		public abstract Task Render(ViewResult viewResult, ViewRenderingContext viewRenderingContext);

		protected virtual UrlHelper BuildUrlHelper(ViewResult result, IView view, ViewRenderingContext context)
		{
			var culture = CultureInfo.CurrentUICulture;
			var regionInfo = culture.IsNeutralCulture ? RegionInfo.CurrentRegion : new RegionInfo(culture.Name);
			return new UrlHelper(context.RequestUrl, result.AreaName, regionInfo.TwoLetterISORegionName, culture.Name, context.ContextItems);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="view"></param>
		/// <param name="viewResult"></param>
		/// <param name="context"></param>
		protected void HydrateView(ViewResult viewResult, IView view, ViewRenderingContext context)
		{
			var urlHelper = this.BuildUrlHelper(viewResult, view, context);

			if (view is IViewWithData)
			{
				var viewWithData = view as IViewWithData;

				if (viewResult.ViewData == null)
					throw new ArgumentNullException("viewData", "No viewdata passed to view `" + view.GetType() + "`");

				var actualViewDataType = viewResult.ViewData.GetType();
				if (!actualViewDataType.IsOrDerivesFrom(viewWithData.ViewDataType))
					throw new ArgumentException("`" + actualViewDataType + "` is not convertible to `" + viewWithData.ViewDataType + "` as defined in the view");

				var htmlHelper = Activator.CreateInstance(typeof(HtmlHelper<>).MakeGenericType(viewWithData.ViewDataType), viewResult, context.RequestUrl, context.RequestState, context.ContextItems) as HtmlHelper;
				
				(htmlHelper as IHtmlHelperT).SetViewData(viewResult.ViewData);

				view.SetHelpers(htmlHelper, urlHelper);
				viewWithData.SetViewData(viewResult.ViewData);
			}
			else
			{
				var htmlHelper = new HtmlHelper(viewResult, context.RequestUrl, this.htmlEncoder, context.RequestState, context.ContextItems);
				view.SetHelpers(htmlHelper, urlHelper);
			}
		}
	}
}
