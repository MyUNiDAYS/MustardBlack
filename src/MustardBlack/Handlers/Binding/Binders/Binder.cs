using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding.Binders
{
	public abstract class Binder : IBinder
	{
		public abstract bool CanBind(string name, Type type, IRequest request, RouteValues routeValues, object owner);

		public BindingResult Bind(string name, Type type, IRequest request, RouteValues routeValues, bool isNestedCall, object owner)
		{
			var nameLower = name.ToLowerInvariant();

			if (routeValues.ContainsKey(nameLower))
			{
				var value = routeValues[nameLower];
				if (value != null)
				{
					var strValue = value.ToString();
					var decodedValue = WebUtility.UrlDecode(strValue);
					return this.GetResult(type, decodedValue, name, request, routeValues, owner);
				}
			}

			if (request.HttpMethod == HttpMethod.Post || request.HttpMethod == HttpMethod.Put)
			{
				if (!string.IsNullOrEmpty(request.ContentType))
				{
					if (request.ContentType.Contains("multipart/form-data"))
					{
						var value = request.Form[name];
						if (value != null)
							return this.GetResult(type, value, name, request, routeValues, owner);
					}
					else if (request.ContentType.Contains("application/x-www-form-urlencoded"))
					{
						var value = request.Form[name];
						if (value != null)
							return this.GetResult(type, value, name, request, routeValues, owner);
					}
					else if (request.ContentType.Contains("application/json"))
					{
						if (request.BufferlessInputStream.Length > 0)
						{
							string jsonString;
							using (var inputStream = new StreamReader(request.BufferlessInputStream))
								jsonString = inputStream.ReadToEnd();

							if (!string.IsNullOrWhiteSpace(jsonString))
								return this.GetResult(type, jsonString, name, request, routeValues, owner);
						}
					}
				}
			}

			var query = request.Url.QueryCollection[name];
			if (query == null)
				return new BindingResult(this.GetDefault(type), BindingResult.ResultType.Default);

			return this.GetResult(type, request.Url.QueryCollection[name], name, request, routeValues, owner);
		}

		public abstract BindingResult Bind(Type type, string name, string value, IRequest request, RouteValues routeValues, object owner);

		BindingResult GetResult(Type type, string value, string name, IRequest request, RouteValues routeValues, object owner)
		{
			try
			{
				return this.Bind(type, name, value, request, routeValues, owner);
			}
			catch (Exception e)
			{
				if (Debugger.IsAttached) Debugger.Break();
				return new BindingResult(null, new BindingError("Invalid", name, value));
			}
		}

		protected virtual object GetDefault(Type type)
		{
			if (type.IsClass)
			{
				if (type != typeof (string))
					return Activator.CreateInstance(type);

				return null;
			}

			if (type.IsValueType)
				return Activator.CreateInstance(type);

			return null;
		}
	}
}