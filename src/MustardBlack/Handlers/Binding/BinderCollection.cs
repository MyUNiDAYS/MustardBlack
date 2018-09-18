using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MustardBlack.Handlers.Binding.Binders;
using MustardBlack.Hosting;
using MustardBlack.Routing;
using NanoIoC;

namespace MustardBlack.Handlers.Binding
{
	public static class BinderCollection
	{
		static readonly ConcurrentDictionary<HashKey, IBinder> cache = new ConcurrentDictionary<HashKey, IBinder>();

		public static IList<IBinder> Binders { get; private set; }

		public static void Initialize(IContainer container)
		{
			Binders = new List<IBinder>
			{
				container.Resolve<Binders.RequestBinder>(),
				container.Resolve<ResponseBinder>(),
				container.Resolve<JsonBinder>(),
				container.Resolve<GuidBinder>(),
				container.Resolve<BoolBinder>(),
				container.Resolve<EnumBinder>(),
				container.Resolve<DateTimeBinder>(),
				container.Resolve<ListBinder>(),
				container.Resolve<FileBinder>(),
				container.Resolve<NullableBinder>(),
				container.Resolve<DictionaryBinder>(),
				container.Resolve<EnumerableOfFileBinder>(),
				container.Resolve<EnumerableOfStringsBinder>(),
				container.Resolve<EnumerableOfEnumerableBinder>(),
				container.Resolve<EnumerableBinder>(),
				container.Resolve<ComplexTypeBinder>(),
				container.Resolve<DefaultBinder>()
			};

		}

		public static IBinder FindBinderFor(string name, Type type, IRequest request, RouteValues routeValues, object owner)
		{
			var key = HashKey.Create(name, type, request, routeValues);
			return cache.GetOrAdd(key, _ => Binders.FirstOrDefault(b => b.CanBind(name, type, request, routeValues, owner)));
		}

		class HashKey : IEquatable<HashKey>
		{
			public string Name { get; }
			public Type Type { get; }
			public string RequestContentType { get; }
			public HttpMethod RequestMethod { get; }
			public RouteValues RouteValues { get; }

			HashKey(string name, Type type, string requestContentType, HttpMethod requestMethod, RouteValues routeValues)
			{
				this.Name = name;
				this.Type = type;
				this.RequestContentType = requestContentType;
				this.RequestMethod = requestMethod;
				this.RouteValues = routeValues;
			}

			public static HashKey Create(string name, Type type, IRequest request, RouteValues routeValues)
			{
				return new HashKey(name, type, request.ContentType.ToString(), request.HttpMethod, routeValues);
			}

			public bool Equals(HashKey other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return string.Equals(this.Name, other.Name) &&
				       this.Type == other.Type &&
				       string.Equals(this.RequestContentType, other.RequestContentType) &&
				       this.RequestMethod == other.RequestMethod &&
				       RouteValuesEquals(this.RouteValues, other.RouteValues);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				return obj.GetType() == this.GetType() &&
				       this.Equals((HashKey)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = (this.Name != null ? this.Name.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (this.Type != null ? this.Type.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (this.RequestContentType != null ? this.RequestContentType.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (int)this.RequestMethod;
					hashCode = (hashCode * 397) ^ GetRouteValuesHashCode(this.RouteValues);
					return hashCode;
				}
			}

			static bool RouteValuesEquals(RouteValues a, RouteValues b)
			{
				return a.Keys.Count == b.Keys.Count &&
				       !a.Keys.Except(b.Keys).Any();
			}

			static int GetRouteValuesHashCode(RouteValues routeValues)
			{
				return routeValues == null || routeValues.Count == 0
					? 0
					: routeValues.Keys.Aggregate(1, (hc, k) => (hc * 397) ^ k.GetHashCode());
			}
		}
	}
}
