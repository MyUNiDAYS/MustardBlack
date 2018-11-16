using System;

namespace MustardBlack.ViewEngines.Razor
{
	public abstract class RazorViewPage<TViewData> : RazorViewPage, IRazorViewPageWithData
	{
		public TViewData ViewData { get; set; }
		
		[Obsolete("Do not use, use `this` instead.")]
		protected new RazorViewPage<TViewData> Html => this;

		[Obsolete("Do not use, use `this` instead.")]
		protected new RazorViewPage<TViewData> Url => this;

		void IRazorViewPageWithData.SetViewData(object viewData)
		{
			this.ViewData = (TViewData) viewData;
		}

		Type IRazorViewPageWithData.ViewDataType => typeof(TViewData);
	}
}