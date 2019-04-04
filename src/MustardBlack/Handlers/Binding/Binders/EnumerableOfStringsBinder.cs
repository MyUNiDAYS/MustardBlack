using System;
using System.Collections.Generic;
using System.Linq;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class EnumerableOfStringsBinder : Binder
	{
		public override bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type == typeof (IEnumerable<string>) && !type.IsArray;
		}

		public override BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			var strValue = value;

			if (string.IsNullOrEmpty(strValue))
				return new BindingResult(new string[0], BindingResult.ResultType.Default);

			var lines = strValue.Split('\n');
			var cols = strValue.Split(',');

			if (cols.Length > lines.Length)
				return new BindingResult(cols);

			var trimmedLines = lines.Select(l => l.Trim('\r')).ToArray();
			return new BindingResult(trimmedLines);
		}
	}
}