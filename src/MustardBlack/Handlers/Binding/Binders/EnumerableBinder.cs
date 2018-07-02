using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class EnumerableBinder : IBinder
	{
		public bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
		}

		public BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner)
		{
			if (request.HttpMethod == HttpMethod.Post || request.HttpMethod == HttpMethod.Put)
			{
				if (request.Form[name] != null)
					return this.Bind(type, name, request.Form[name], request, routeValues, owner);
			}

			var query = request.Url.QueryCollection[name];
			if (query != null)
				return this.Bind(type, name, request.Url.QueryCollection[name], request, routeValues, owner);

			var innerType = GetEnumerableInnerType(type);
			return new BindingResult(new object[0].Cast(innerType));
		}

		public BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			var bindingErrors = new List<BindingError>();

			var innerType = GetEnumerableInnerType(type);

			var list = new List<object>();

            var innerBinder = BinderCollection.FindBinderFor(null, innerType, request, routeValues, owner);

			var stringValue = value;
			if (!string.IsNullOrEmpty(stringValue))
			{
				var strings = stringValue.Split(innerType == typeof(string) ? '\n' : ',');
				foreach (var str in strings)
				{
					var bindingResult = innerBinder.Bind(innerType, name, str, request, routeValues, owner);

					if (bindingResult.Result != BindingResult.ResultType.Failure)
						list.Add(bindingResult.Object);
					else
						bindingErrors.AddRange(bindingResult.BindingErrors);
				}
			}

			var enumerable = list.Cast(innerType);
			return new BindingResult(enumerable, bindingErrors.ToArray(), bindingErrors.Any() ? BindingResult.ResultType.Failure : BindingResult.ResultType.Success);
		}

		static Type GetEnumerableInnerType(Type type)
		{
			if (type.IsGenericType)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();

				if (typeof (IEnumerable<>) == genericTypeDefinition)
					return type.GetGenericArguments()[0];
			}

			throw new NotSupportedException("Binding for property type `" + type.AssemblyQualifiedName + "` is not currently supported. :(");
		}
	}
}