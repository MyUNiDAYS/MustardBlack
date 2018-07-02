using System.Collections.Generic;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	public sealed class HtmlHelper<TViewData> : HtmlHelper, IHtmlHelperT
	{
		public HtmlHelper(ViewResult viewResult, Url requestUrl, IRequestState requestState, IDictionary<string, object> contextItems) : base(viewResult, requestUrl, requestState, contextItems)
		{
		}

		public TViewData ViewData { get; private set; }
		
		void IHtmlHelperT.SetViewData(object viewData)
		{
			if(viewData != null)
				this.ViewData = (TViewData)viewData;
		}
	}
}
