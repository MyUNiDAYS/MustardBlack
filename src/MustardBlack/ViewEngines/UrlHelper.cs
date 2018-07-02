using System;
using System.Collections.Generic;

namespace MustardBlack.ViewEngines
{
	public sealed class UrlHelper
	{
		readonly Url requestUrl;
		public readonly string AreaName;

		public readonly string CurrentRegionCode;
		public readonly string CurrentCultureCode;

		public IDictionary<string, object> ContextItems { get; }

		public Url RequestUrl
		{
			get
			{
				if (this.requestUrl != null)
					return this.requestUrl;

				throw new InvalidOperationException("The RequestUrl is not currently available.");
			}
		}

		public UrlHelper(Url requestUrl, string areaName, string currentRegionCode, string currentCultureCode, IDictionary<string, object> contextItems)
		{
			this.requestUrl = requestUrl;
			this.AreaName = areaName;
			this.CurrentCultureCode = currentCultureCode;
			this.CurrentRegionCode = currentRegionCode;
			this.ContextItems = contextItems;
		}
	}
}
