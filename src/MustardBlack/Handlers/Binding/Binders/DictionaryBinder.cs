using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public class DictionaryBinder : IBinder
	{
		public bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			if (type.IsGenericType && type.GenericTypeArguments.Length == 2)
			{
				var dictType = typeof(IDictionary<,>).MakeGenericType(type.GenericTypeArguments);
				return dictType.IsAssignableFrom(type);
			}
			return false;
		}

		public BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner)
		{
			if (request.HttpMethod == HttpMethod.Post || request.HttpMethod == HttpMethod.Put)
				return this.Bind(type, name, request.Form[name], request, routeValues, owner);

			return new BindingResult(null, BindingResult.ResultType.Failure);
		}

		public BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			var bindingErrors = new List<BindingError>();

			var keyType = GetGenericType(type, 0);
			var valueType = GetGenericType(type, 1);

			var keyBinder = BinderCollection.FindBinderFor(null, keyType, request, routeValues, owner);
			var valueBinder = BinderCollection.FindBinderFor(null, valueType, request, routeValues, owner);

			Regex dictKeyRegex = new Regex("^" + Regex.Escape(name) + "\\[([^]]+)\\]");

			var matchedKeys = request.Form.AllKeys.Select(x => dictKeyRegex.Match(x)).Where(r => r.Success).Distinct(x => x.Value).ToArray();

			var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
			var dict = Activator.CreateInstance(dictType) as IDictionary;

			foreach (var matchedKey in matchedKeys)
			{
				var dictKeyPart = matchedKey.Groups[1].Value;
				var formKey = matchedKey.Value;

				var keyBinding = keyBinder.Bind(keyType, formKey, dictKeyPart, request, routeValues, owner);
				var valueBinding = valueBinder.Bind(formKey, valueType, request, routeValues, true, owner);

				if (keyBinding.Result == BindingResult.ResultType.Success && valueBinding.Result == BindingResult.ResultType.Success)
				{
					dict.Add(keyBinding.Object, valueBinding.Object);
				}
				else
				{
					bindingErrors.AddRange(keyBinding.BindingErrors);
					bindingErrors.AddRange(valueBinding.BindingErrors);
				}
			}

			return new BindingResult(dict, bindingErrors.ToArray(), bindingErrors.Any() ? BindingResult.ResultType.Failure : BindingResult.ResultType.Success);
		}

		static Type GetGenericType(Type type, int index)
		{
			if (type.IsGenericType)
			{
				var genericTypeDefinition = type.GetGenericTypeDefinition();

				if (genericTypeDefinition.IsOrDerivesFrom(typeof(IDictionary<,>)))
					return type.GetGenericArguments()[index];
			}

			throw new NotSupportedException("Binding for property type `" + type.AssemblyQualifiedName + "` is not currently supported. :(");
		}
	}
}