using System.Web;

namespace MustardBlack.ViewEngines
{
	interface IHtmlHelper
	{
		IHtmlString Raw(object value);
	}

	interface IHtmlHelperT : IHtmlHelper
	{
		void SetViewData(object viewData);
	}
}
