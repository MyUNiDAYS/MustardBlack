using System;
using System.Net;

namespace MustardBlack.Results
{
	public sealed class ViewResult : Result
	{
		public Type ViewType { get; }
		public string AreaName { get; }
		public object ViewData { get; }
		
		public ViewResult(Type viewType, string areaName, object viewData, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			this.ViewType = viewType;
			this.AreaName = areaName;
			this.StatusCode = statusCode;
			this.ViewData = viewData;
		}
	}
}
