using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	static class EnumerableExtensions
	{
		/// <summary>
		/// Casts each item in the enumerable to the given type
		/// </summary>
		/// <param name="self"></param>
		/// <param name="innerType"></param>
		/// <returns></returns>
		public static IEnumerable Cast(this IEnumerable self, Type innerType)
		{
			var methodInfo = typeof(Enumerable).GetMethod("Cast");
			var genericMethod = methodInfo.MakeGenericMethod(innerType);
			return genericMethod.Invoke(null, new[] { self }) as IEnumerable;
		}

		/// <summary>
		/// Clones the IEnumerable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="self"></param>
		/// <returns></returns>
		public static IEnumerable<T> Clone<T>(this IEnumerable<T> self)
		{
			return new List<T>(self);
		}
		

		public static IEnumerable<TIn> Distinct<TIn, TProp>(this IEnumerable<TIn> enumerable, Func<TIn, TProp> property) where TProp : IComparable
		{
			var comparer = new TEqualityComparer<TIn, TProp>(property);
			return enumerable.Distinct(comparer);
		}

		sealed class TEqualityComparer<T, TProp> : IEqualityComparer<T> where TProp : IComparable
		{
			readonly Func<T, TProp> property;

			public TEqualityComparer(Func<T, TProp> property)
			{
				this.property = property;
			}

			public bool Equals(T x, T y)
			{
				return this.property(x).Equals(this.property(y));
			}

			public int GetHashCode(T obj)
			{
				return this.property(obj).GetHashCode();
			}
		}


	}
}