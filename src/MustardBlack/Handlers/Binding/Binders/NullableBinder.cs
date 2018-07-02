using System;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class NullableBinder : Binder
	{
        public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			var underlyingType = Nullable.GetUnderlyingType(type);

			if (string.IsNullOrEmpty(value))
				return new BindingResult(null, BindingResult.ResultType.Default);

			var binder = BinderCollection.FindBinderFor(name, underlyingType, request, routeValues, owner);
			return binder.Bind(type, name, value, request, routeValues, owner);
		}
	}
}