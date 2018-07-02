using System;
using MustardBlack.Hosting;
using MustardBlack.Routing;
using NanoIoC;

namespace MustardBlack.Handlers.Binding.Binders
{
	// Antipattern, replace usages with this.Context.Response
	sealed class ResponseBinder : IBinder
	{
	    readonly IContainer container;

	    public ResponseBinder(IContainer container)
	    {
	        this.container = container;
	    }

	    public bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type.IsOrDerivesFrom(typeof (IResponse));
		}

		public BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner)
		{
			return new BindingResult(this.container.Resolve<IResponse>());	
		}

		public BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			return new BindingResult(this.container.Resolve<IResponse>());	
		}
	}
}