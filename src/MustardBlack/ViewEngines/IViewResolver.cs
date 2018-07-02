using System;

namespace MustardBlack.ViewEngines
{
	public interface IViewResolver
	{
		Type Resolve(string viewName);
	}
}
