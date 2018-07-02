using System;

namespace MustardBlack.ViewEngines
{
	public interface IViewLocator
	{
		Type Locate(string viewPath);
	}
}
