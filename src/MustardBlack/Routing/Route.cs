using System;
using System.Diagnostics;
using System.Globalization;

namespace MustardBlack.Routing
{
	[DebuggerDisplay("Route: {" + nameof(Path) + "}")]
	public class Route : IRoute
	{
		protected readonly string areaName;
		readonly ParsedRoute parsedRoute;
		readonly ParsedRoute parsedLocalisedRoute;

		/// <summary>
		/// The route that is handled
		/// </summary>
		public Type HandlerType { get; }

		/// <summary>
		/// The route that is handled
		/// </summary>
		public string Path { get; set; }
		
		/// <summary>
		/// The request types handled by this route
		/// </summary>
		public RequestType RequestTypes { get; set; }

		public HttpMethod HandledMethods { get; set; }

		/// <summary>
		/// Indicates if the route is localised
		/// </summary>
		public bool Localised { get; }
		/// <summary>
		/// Indicates if the route is personalised
		/// </summary>
		public bool Personalised { get; }
		
		public Route(string url, string areaName, Type handlerType, bool localised, bool personalised)
		{
			this.areaName = areaName;
			this.HandlerType = handlerType;
			this.Localised = localised;
			this.Personalised = personalised;

			if (string.IsNullOrEmpty(url))
				throw new ArgumentException("URL must have a value", nameof(url));

			if (!url.StartsWith("/"))
				throw new ArgumentException("URL `" + url + "` must start with a /", nameof(url));

			this.Path = url;
			this.parsedRoute = ParsedRoute.Parse(url);

			if (localised)
			{
				if(url != "/")
					this.parsedLocalisedRoute = ParsedRoute.Parse("/{regionCode}/{cultureCode}/" + url.TrimStart('/'));
				else
					this.parsedLocalisedRoute = ParsedRoute.Parse("/{regionCode}/{cultureCode}");
			}
		}

		public virtual RouteValues GetRouteValues(Url url, HttpMethod method, RequestType requestType)
		{
			if (this.HandledMethods != 0 && (this.HandledMethods & method) != method)
				return null;

			if (this.RequestTypes != 0 && this.RequestTypes != requestType)
				return null;
			
			RouteValues routeValues = null;

			// Check localised route first, otherwise localised wildcard and other routes of the form "/{param}" can accidentally match
            if (this.Localised)
			{
				routeValues = this.parsedLocalisedRoute.Match(url.Path);

				// Simple check to prevent most route collisions with /{regionCode}/{cultureCode}
				// If the regionCode isnt 2 chars, we almost certainly shouldn't be routed here
				var regionCode = (string)routeValues?["regionCode"];
				if (regionCode != null && regionCode.Length != 2)
					return null;
			}

			if (routeValues == null)
				routeValues = this.parsedRoute.Match(url.Path);

			if (routeValues != null)
				routeValues["AreaName"] = this.areaName;

			return routeValues;
		}

		public virtual string BuildPath(RouteValues values = null, bool localised = false)
		{
			var url = localised ? this.parsedLocalisedRoute.BuildPath(values) : this.parsedRoute.BuildPath(values);
			return url;
		}
		
		public string BuildLocalisedPath(RouteValues values, string regionCode, CultureInfo cultureCode)
		{
			if(this.parsedLocalisedRoute == null)
				throw new InvalidOperationException($"This route `{this.Path}` is not localisable");

			var routeValues = new RouteValues(values);
			routeValues["regionCode"] = regionCode;
			routeValues["cultureCode"] = cultureCode.Name;

			return this.parsedLocalisedRoute.BuildPath(routeValues);
		}
	}
}
