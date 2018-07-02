using System;
using System.Globalization;

namespace MustardBlack.Routing
{
	public interface IRoute
	{
		RouteValues GetRouteValues(Url url, HttpMethod method, RequestType requestType);

		/// <summary>
		/// If you want localised, pass regionCode and cultureCode in as routeValues
		/// </summary>
		/// <param name="values"></param>
		/// <param name="localised"></param>
		/// <returns></returns>
		string BuildPath(RouteValues values, bool localised = false);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="values"></param>
		/// <param name="regionCode">Overrides a regionCode in routeValues</param>
		/// <param name="cultureCode">Overrides a cultureCode in routeValues</param>
		/// <returns></returns>
		string BuildLocalisedPath(RouteValues values, string regionCode, CultureInfo cultureCode);

		/// <summary>
		/// The route that is handled
		/// </summary>
		Type HandlerType { get; }

		/// <summary>
		/// The route that is handled
		/// </summary>
		string Path { get; }

		/// <summary>
		/// The request types handled by this route
		/// </summary>
		RequestType RequestTypes { get; set; }

		HttpMethod HandledMethods { get; set; }

		/// <summary>
		/// Indicates if the route is localised
		/// </summary>
		bool Localised { get; }

		/// <summary>
		/// Indicates if the route is personalised
		/// </summary>
		bool Personalised { get; }
	}
}
