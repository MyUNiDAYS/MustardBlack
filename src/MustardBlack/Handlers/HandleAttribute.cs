using System;
using System.Diagnostics;
using System.Reflection;
using MustardBlack.Routing;
using Serilog;

namespace MustardBlack.Handlers
{
	/// <summary>
	/// Describes a route that the marked Handler can handle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class HandleAttribute : Attribute
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public RouteDefinition Route { get; }

		/// <summary>
		/// Creates a new handle attribute for the given route.
		/// Should only be used by the Admin
		/// </summary>
		/// <param name="route">The route to handle</param>
		public HandleAttribute(string route)
		{
			this.Route = new RouteDefinition(route)
			{
				Localised = false
			};
		}
		
		public HandleAttribute(Type type, RequestType requestTypes = 0)
		{
			var nestedTypes = type.GetNestedTypes();
			if (nestedTypes.Length == 0)
			{
				if (type.IsOrDerivesFrom<RouteDefinition>())
				{
					try
					{
						this.Route = Activator.CreateInstance(type) as RouteDefinition;
						this.Route.RequestTypes = requestTypes;
					}
					catch (Exception e)
					{
						if (Debugger.IsAttached) Debugger.Break();
						log.Error(e, "Cannot handle route defined by {type}", type.FullName);
					}
				}
				else
				{
					log.Error("Route definition {type} does not implement " + typeof(RouteDefinition).Name, type);
				}
			}
			else
			{
				log.Error("Route definition {type} does not implement " + typeof(RouteDefinition).Name + ". Is your [Handle(...)] namespace/path correct?", type);
			}
		}
	}
}