namespace MustardBlack.ViewEngines
{
	interface IHtmlHelper
	{
	}

	interface IHtmlHelperT : IHtmlHelper
	{
		void SetViewData(object viewData);
	}
}
