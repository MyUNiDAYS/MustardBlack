using System;
using System.Collections.Generic;
using System.Linq;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class EnumerableOfEnumerableBinder : Binder
	{
		public override bool CanBind(string name, Type outerType, IRequest request, RouteValues routeValues, object owner)
		{
			if (!outerType.IsGenericType)
				return false;

			var genericOuterType = outerType.GetGenericTypeDefinition();

			if (genericOuterType != typeof(IEnumerable<>))
				return false;
			
			var middleType = outerType.GetGenericArguments()[0];

			if (!middleType.IsGenericType)
				return false;

			var genericMiddleType = middleType.GetGenericTypeDefinition();

			if (genericMiddleType != typeof(IEnumerable<>))
				return false;

			var innerType = middleType.GetGenericArguments()[0];

            var innerBinder = BinderCollection.FindBinderFor(null, innerType, request, routeValues, owner);

			return innerBinder != null;
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			var middleType = type.GetGenericArguments()[0];
			var innerType = middleType.GetGenericArguments()[0];
            var innerBinder = BinderCollection.FindBinderFor(null, innerType, request, routeValues, owner);

			var bound = new List<List<object>>();
			var strValue = value;

			if (!string.IsNullOrEmpty(strValue))
			{
				var rows = strValue.Split('\n');

				foreach (var row in rows)
				{
					var cleanRow = row.Trim();

					if (cleanRow == string.Empty)
						continue;

					var newRow = new List<object>();

					var cells = cleanRow.Split(',');

					foreach (var cell in cells)
					{
						var bindingResult = innerBinder.Bind(innerType, name, cell, request, routeValues, owner);

						if (bindingResult.Result != BindingResult.ResultType.Failure)
							newRow.Add(bindingResult.Object);
						else
							throw new NotImplementedException();
					}
					bound.Add(newRow);
				}
			}

			var enumerable = bound.Select(inner => inner.Cast(innerType));
			var result = enumerable.Cast(typeof(IEnumerable<>).MakeGenericType(innerType));

			return new BindingResult(result);
		}
	}
}