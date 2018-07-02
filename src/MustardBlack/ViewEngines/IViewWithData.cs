using System;

namespace MustardBlack.ViewEngines
{
	public interface IViewWithData : IView
	{
		void SetViewData(object viewData);
		Type ViewDataType { get; }
	}
}