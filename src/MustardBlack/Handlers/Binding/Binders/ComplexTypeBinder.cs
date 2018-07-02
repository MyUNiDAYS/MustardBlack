using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class ComplexTypeBinder : IBinder
	{
		public bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			if (!type.IsClass)
				return false;

			if (!type.Attributes.HasFlag(TypeAttributes.Sealed))
				type = GetTypeFromTypeSpecifier(string.IsNullOrEmpty(name) ? "$Type" : name + ".$Type", request, routeValues, owner) ?? type;

			if (type.IsAbstract)
				return false;

			return GetCtor(type) != null;
		}

		public BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner)
		{
			var thingType = GetTypeFromTypeSpecifier(isNestedCall ? name + ".$Type" : "$Type", request, routeValues, owner) ?? type;

			var ctor = GetCtor(thingType);
			var instance = ctor.Invoke(new object[0]);

			var bindingErrors = SetProperties(isNestedCall ? name : null, thingType, instance, request, routeValues, owner).ToArray();

			return new BindingResult(instance, bindingErrors, bindingErrors.Any() ? BindingResult.ResultType.Failure : BindingResult.ResultType.Success);
		}

		static IEnumerable<BindingError> SetProperties(string prefix, Type type, object instance, IRequest request, RouteValues routeValues, object owner)
		{
			var bindingErrors = new List<BindingError>();

			var settableProperties = GetSettableProperties(type);

			foreach (var property in settableProperties)
			{
				var typeSpecifierName = (prefix != null ? prefix + "." + property.Name : property.Name) + ".$Type";
				var propertyType = GetTypeFromTypeSpecifier(typeSpecifierName, request, routeValues, owner) ?? property.PropertyType;

				var binder = BinderCollection.FindBinderFor(property.Name, propertyType, request, routeValues, owner);

				if (binder == null)
					continue;

				var name = prefix != null ? prefix + "." + property.Name : property.Name;

				var bindingResult = binder.Bind(name, property.PropertyType, request, routeValues, true, owner);

				property.SetValue(instance, bindingResult.Object, null);

				if (bindingResult.Result == BindingResult.ResultType.Failure)
					bindingErrors.AddRange(bindingResult.BindingErrors);
			}

			return bindingErrors;
		}

		static Type GetTypeFromTypeSpecifier(string name, IRequest request, RouteValues routeValues, object owner)
		{
			var binder = BinderCollection.FindBinderFor(name, typeof(string), request, routeValues, owner);

			if (binder == null)
				return null;

			var bindingResult = binder.Bind(name, typeof(string), request, routeValues, true, owner);

			if (bindingResult.Result == BindingResult.ResultType.Success && bindingResult.Object as string != null)
				return Type.GetType((string)bindingResult.Object, false) ?? null;

			return null;
		}

		static IEnumerable<PropertyInfo> GetSettableProperties(Type type)
		{
			var propertyInfos = type.GetProperties();
			return propertyInfos.Where(p => p.GetSetMethod() != null);
		}

		public BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			throw new NotSupportedException("This shouldn't be called");
		}

		static ConstructorInfo GetCtor(Type type)
		{
			return type.GetConstructor(Type.EmptyTypes);
		}
	}
}
