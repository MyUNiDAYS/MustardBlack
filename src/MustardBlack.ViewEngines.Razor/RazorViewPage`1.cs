using System;

namespace MustardBlack.ViewEngines.Razor
{
	public abstract class RazorViewPage<TViewData> : RazorViewPage, IViewWithData
	{
		protected TViewData ViewData { get; private set; }
		protected new HtmlHelper<TViewData> Html => base.Html as HtmlHelper<TViewData>;
		HtmlHelper IView.Html => this.Html;
		public Type ViewDataType => typeof (TViewData);

		public void SetViewData(object viewData)
		{
			if(viewData != null)
				this.ViewData = (TViewData) viewData;
		}
	}
}