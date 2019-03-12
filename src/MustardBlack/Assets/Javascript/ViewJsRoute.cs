using System;
using System.Globalization;
using MustardBlack.Assets.Static;
using MustardBlack.Routing;

namespace MustardBlack.Assets.Javascript
{
	sealed class ViewJsRoute : IRoute
	{
		readonly string pathPrefix;
		public Type HandlerType { get; }
		public string Path => "/{*path}.js";
		public RequestType RequestTypes { get; set; }
		public HttpMethod HandledMethods { get; set; }
		bool IRoute.Localised => false;
		bool IRoute.Personalised => false;

		public ViewJsRoute(string pathPrefix)
		{
			this.pathPrefix = pathPrefix.ToLowerInvariant();
			this.HandlerType = typeof(AreaStaticAssetHandler);
			this.HandledMethods = HttpMethod.Get | HttpMethod.Head | HttpMethod.Options;
		}

		public RouteValues GetRouteValues(Url url, HttpMethod method, RequestType requestType)
		{
			var path = url.Path.ToLowerInvariant();

			if (path.EndsWith(".js") && path.StartsWith(this.pathPrefix))
				return new RouteValues();

			return null;
		}

		string IRoute.BuildPath(RouteValues values, bool localised = false)
		{
			throw new NotImplementedException();
		}

		string IRoute.BuildLocalisedPath(RouteValues values, string regionCode, CultureInfo cultureCode)
		{
			throw new NotImplementedException();
		}
	}
}