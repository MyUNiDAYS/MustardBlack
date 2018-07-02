using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MustardBlack.Assets.Css;
using MustardBlack.Assets.Javascript;
using MustardBlack.Handlers;
using MustardBlack.Routing;
using Serilog;

namespace MustardBlack.Areas
{
	public abstract class AreaRegistrationBase
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
		public string AreaName => this.GetType().Namespace.Substring(this.GetType().Namespace.LastIndexOf('.') + 1);

		public virtual void RegisterArea(IHandlerCache handlerCache, ICollection<IRoute> routeCollection)
		{
			var handlers = this.FindHandlers(handlerCache);

			var hash = this.HashRoutes(handlers);

			// TODO: only include these when in dev mode
			this.MapRoute(routeCollection, "/" + this.AreaName + ".css", typeof(AreaCssHandler), false, false);
			this.MapRoute(routeCollection, "/" + this.AreaName + ".js", typeof(AreaJavascriptHandler), false, false);

			foreach (var routeRegistration in hash)
				this.RegisterRoute(handlerCache, routeCollection, routeRegistration);
		}

		void RegisterRoute(IHandlerCache handlerCache, ICollection<IRoute> routeCollection, KeyValuePair<string, List<Tuple<Type, RouteDefinition>>> routeRegistration)
		{
			// If there is only a single handler for this route
			if (routeRegistration.Value.Count == 1)
			{
				this.RegisterHandler(routeCollection, routeRegistration.Value[0].Item1, routeRegistration.Key, routeRegistration.Value[0].Item2.Localised, routeRegistration.Value[0].Item2.Personalised, routeRegistration.Value[0].Item2.RequestTypes, 0);
				return;
			}

			// Else we have mutliple handlers for this route

			var handlersAndMethods = new List<Tuple<Type, RouteDefinition, HttpMethod>>();
			foreach (var handler in routeRegistration.Value)
			{
				var handledMethods = handlerCache.GetHandledMethods(handler.Item1);
				handlersAndMethods.Add(new Tuple<Type, RouteDefinition, HttpMethod>(handler.Item1, handler.Item2, handledMethods));
			}

			var orderedEnumerable = handlersAndMethods.OrderBy(x => (x.Item3 != 0 ? "0" : "1") + (x.Item2.RequestTypes > 0 ? "0" : "1"));

			foreach (var handler in orderedEnumerable)
				this.RegisterHandler(routeCollection, handler.Item1, routeRegistration.Key, handler.Item2.Localised, handler.Item2.Personalised, handler.Item2.RequestTypes, handler.Item3);
		}

		void RegisterHandler(ICollection<IRoute> routeCollection, Type handlerType, string route, bool localised, bool personalised, RequestType requestTypes, HttpMethod handledMethods)
		{
			log.Debug("Registering handler {handler} for {route}", handlerType, route);

			var mappedRoute = this.MapRoute(routeCollection, route, handlerType, localised, personalised);
			if (mappedRoute != null)
			{
				mappedRoute.RequestTypes = requestTypes;
				mappedRoute.HandledMethods = handledMethods;
			}
		}
		IEnumerable<Type> FindHandlers(IHandlerCache handlerCache)
		{
			var thisAssmebly = this.GetType().Assembly;

			var handlers = handlerCache.AllHandlerTypes.Where(handler =>
			{
				if (handler.IsAbstract)
					return false;

				if (handler.Assembly != thisAssmebly)
					return false;

				// Don't register handlers from other areas
				if (handler.Namespace.Contains(".Areas.") && !handler.Namespace.Contains(".Areas." + this.AreaName + ".") && !handler.Namespace.Contains(".Areas.Common."))
					return false;

				return true;
			}).ToArray();

#if DEBUG
			var handlerNames = string.Join("\n", handlers.Select(h => h.FullName));
			log.Debug("Found {handlerCount} handler(s) for {Area}\n{Handlers}", handlers.Length, this.AreaName, handlerNames);
#endif

			return handlers;
		}

		Dictionary<string, List<Tuple<Type, RouteDefinition>>> HashRoutes(IEnumerable<Type> handlers)
		{
			var hash = new Dictionary<string, List<Tuple<Type, RouteDefinition>>>();

			foreach (var handler in handlers)
			{
				object[] handleAttributes;
				try
				{
					handleAttributes = handler.GetCustomAttributes(typeof(HandleAttribute), true);
				}
				catch (Exception e)
				{
					log.Error(e, "Unable to construct HandleAttributes for Handler {handler}, skipping Handler registration", handler);
					continue;
				}

				foreach (HandleAttribute attribute in handleAttributes)
				{
					if (attribute == null)
						log.Error("Handler {handler} has no [Handle] attributes", handler.FullName);
					else if (attribute.Route == null)
						log.Error("Null route found for handler: {handler}. There should be an accompanying Error log entry for the cause of this. Here is the symptom.", handler.FullName);
					else
						HashRoute(attribute, hash, handler);
				}
			}

			return hash;
		}

		static void HashRoute(HandleAttribute attribute, IDictionary<string, List<Tuple<Type, RouteDefinition>>> hash, Type handler)
		{
			try
			{
				if (!hash.ContainsKey(attribute.Route.RoutePattern))
					hash.Add(attribute.Route.RoutePattern, new List<Tuple<Type, RouteDefinition>>());
			}
			catch (Exception e)
			{
				log.Error(e, "Failed to hash Route for {handler} (Dont know how to be more helpful, attach the debugger)", handler);
				if (Debugger.IsAttached) Debugger.Break();
				return;
			}

			hash[attribute.Route.RoutePattern].Add(new Tuple<Type, RouteDefinition>(handler, attribute.Route));
		}

		protected virtual IRoute MapRoute(ICollection<IRoute> routeCollection, string url, Type handlerType, bool localised, bool personalised)
		{
			var route = new Route(url, this.AreaName, handlerType, localised, personalised);
			routeCollection.Add(route);

			return route;
		}
	}
}
