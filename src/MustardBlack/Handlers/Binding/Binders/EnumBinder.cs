using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public class EnumBinder : Binder
	{
		static readonly ConcurrentDictionary<Type,bool> cache = new ConcurrentDictionary<Type, bool>();

		public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type.IsEnum || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>) && Nullable.GetUnderlyingType(type).IsEnum);
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			var conv = TypeDescriptor.GetConverter(type);

			if (string.IsNullOrEmpty(value) || HasCustomAttributes(type))
				return new BindingResult(conv.ConvertFrom(value.Replace("-", string.Empty)));

			var values = value;
			var strings = values.Split(',');

			// TODO: support ints more than

			int i = 0;
			foreach (var val in strings)
				i |= (int) conv.ConvertFrom(val.Replace("-", string.Empty));

			return new BindingResult(i);
		}

		static bool HasCustomAttributes(Type type)
		{
			return cache.GetOrAdd(type, _ => type.GetCustomAttributes(typeof (FlagsAttribute), true).Length == 0);
		}
	}
}
