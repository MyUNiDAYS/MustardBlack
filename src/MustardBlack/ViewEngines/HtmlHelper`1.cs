using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.ViewEngines
{
	public sealed class HtmlHelper<TViewData> : HtmlHelper, IHtmlHelperT
	{
		public HtmlHelper(ViewResult viewResult, Url requestUrl, HtmlEncoder encoder, IRequestState requestState, IDictionary<string, object> contextItems, TextWriter writer) : base(viewResult, requestUrl, encoder, requestState, contextItems, writer)
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
