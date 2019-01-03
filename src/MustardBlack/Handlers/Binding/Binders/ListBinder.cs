using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class ListBinder : IBinder
	{
		public bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type.IsOrDerivesFrom(typeof(IList<>)) && !type.IsArray;
		}

		public BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner)
		{
			var prefix = name + "[";
			var bindingErrors = new List<BindingError>();

			if (request.HttpMethod == HttpMethod.Post || request.HttpMethod == HttpMethod.Put || request.HttpMethod == HttpMethod.Patch)
			{
				if (request.ContentType != null && (request.ContentType.MediaType == "multipart/form-data" || request.ContentType.MediaType == "application/x-www-form-urlencoded"))
				{
					var genericArgument = type.GetGenericArguments()[0];

					var list = Activator.CreateInstance(typeof(ExpandableList<>).MakeGenericType(genericArgument)) as IList;

					foreach (var key in request.Form.AllKeys)
					{
						if (key.StartsWith(prefix, true, CultureInfo.InvariantCulture))
						{
							var start = prefix.Length;
							var end = key.IndexOf(']', start);

							var index = int.Parse(key.Substring(start, end - start));

							var indexedName = name + "[" + index + "]";
							var innerBinder = BinderCollection.FindBinderFor(indexedName, genericArgument, request, routeValues, owner);
							var bind = innerBinder.Bind(indexedName, genericArgument, request, routeValues, true, owner);

							list[index] = bind.Object;

							if (bind.Result == BindingResult.ResultType.Failure)
								bindingErrors.AddRange(bind.BindingErrors);
						}
					}

					return new BindingResult(list, bindingErrors.ToArray(), bindingErrors.Any() ? BindingResult.ResultType.Failure : BindingResult.ResultType.Success);
				}
			}

			throw new NotImplementedException("Cannot bind a List from the query string");
		}

		public BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			throw new NotImplementedException();
		}
	}
}
