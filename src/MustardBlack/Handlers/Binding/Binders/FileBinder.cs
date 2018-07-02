using System;
using System.Linq;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public sealed class FileBinder : IBinder
	{
		public bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			return type.IsOrDerivesFrom(typeof(IFile));
		}

		public BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner)
		{
			if(request.Files.ContainsKey(name))
			{
				var enumerable = request.Files[name];
				return new BindingResult(enumerable.First());
			}

			return new BindingResult(null, BindingResult.ResultType.Default);
		}

		public BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner)
		{
			return new BindingResult(value);
		}
	}
}