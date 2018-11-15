using System;

namespace MustardBlack.ViewEngines.Razor
{
	public interface IRazorViewPageWithData
	{
		void SetViewData(object viewData);
		Type ViewDataType { get; }
	}
}