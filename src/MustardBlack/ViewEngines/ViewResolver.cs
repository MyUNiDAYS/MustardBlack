using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog;

namespace MustardBlack.ViewEngines
{
	sealed class ViewResolver : IViewResolver
	{
		readonly IEnumerable<IViewLocator> viewLocators;

		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public ViewResolver(IEnumerable<IViewLocator> viewLocators)
		{
			this.viewLocators = viewLocators;

			log.Debug("Initialising ViewResolver with {count} view locators: {locators}", viewLocators.Count(), viewLocators.Select(l => l.GetType().FullName).ToArray());
		}

		public Type Resolve(string viewName)
		{
			Type viewType = null;
			foreach (var locator in this.viewLocators)
			{
				log.Debug("Searching for View {viewName} using {locator}", viewName, locator.GetType());

				viewType = locator.Locate(viewName);
				if (viewType != null)
				{
					log.Debug("Found View {viewName} within {requestingNamespace} using {locator}", viewName, locator.GetType());
					break;
				}
			}

			if (viewType == null)
				throw new ViewLocationException("Could not locate view `" + viewName + "`");

			return viewType;
		}
	}
}
