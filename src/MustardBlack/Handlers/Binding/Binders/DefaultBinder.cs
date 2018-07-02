using System;
using System.ComponentModel;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class DefaultBinder : Binder
	{
		public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			var convertor = TypeDescriptor.GetConverter(type);
			if(convertor == null)
				return false;

			return convertor.CanConvertFrom(typeof (string));
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (value == null)
					return new BindingResult(this.GetDefault(type), BindingResult.ResultType.Default);

				type = type.GetGenericArguments()[0];
			}
			else
			{
				if (value == null)
					return new BindingResult(this.GetDefault(type), BindingResult.ResultType.Default);
			}

			try
			{
				var converter = TypeDescriptor.GetConverter(type);
				var converted = converter.ConvertFrom(value);
				return new BindingResult(converted);
			}
			catch (Exception e)
			{
				var val = type.IsPrimitive ? Activator.CreateInstance(type) : null;
                return new BindingResult(val, new BindingError("Invalid", name, value));
			}
		}
	}
}