using System;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding
{
	public interface IBinder
	{
		bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner);
		BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner);
		BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner);
	}
}