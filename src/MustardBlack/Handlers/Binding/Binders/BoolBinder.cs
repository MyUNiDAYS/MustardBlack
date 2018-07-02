using System;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class BoolBinder : Binder
	{
		public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type == typeof(bool);
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			if (string.IsNullOrWhiteSpace(value))
				return new BindingResult(false, BindingResult.ResultType.Default);

			// TODO: this is a bug waiting to happen. Check for "" vs "true,false" or similar
			return new BindingResult(value.ToUpperInvariant().Contains("TRUE"));
		}
	}
}