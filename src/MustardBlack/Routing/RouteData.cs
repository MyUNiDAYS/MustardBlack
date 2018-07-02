using MustardBlack.Handlers;

namespace MustardBlack.Routing
{
	public sealed class RouteData
	{
		public IRoute Route { get; private set; }
		public RouteValues Values { get; private set; }
		public HandlerAction HandlerAction { get; set; }

		public RouteData(IRoute route, RouteValues values)
		{
			this.Route = route;
			this.Values = values;
		}

		public RouteData(IRoute route) : this(route, new RouteValues())
		{
		}
	}
}
