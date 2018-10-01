using Microsoft.AspNetCore.Html;

namespace MustardBlack.ViewEngines
{
	interface IHtmlHelper
	{
		IHtmlContent Raw(object value);
	}

	interface IHtmlHelperT : IHtmlHelper
	{
		void SetViewData(object viewData);
	}
}
