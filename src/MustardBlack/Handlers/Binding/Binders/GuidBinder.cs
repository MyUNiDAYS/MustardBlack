using System;
using System.ComponentModel;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class GuidBinder : Binder
	{
		static readonly TypeConverter converter = TypeDescriptor.GetConverter(typeof(Guid));

		public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type == typeof(Guid);
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			if (string.IsNullOrEmpty(value))
				return new BindingResult(Guid.Empty, BindingResult.ResultType.Default);

			try
			{
				var converted = converter.ConvertFrom(value);
				return new BindingResult(converted);
			}
			catch(FormatException e)
			{
				return new BindingResult(Guid.Empty, new BindingError(e.Message, name, value));
			}
		}
	}
}
