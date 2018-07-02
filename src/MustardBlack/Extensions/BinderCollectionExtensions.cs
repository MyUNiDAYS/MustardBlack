using System;
using System.Collections.Generic;
using MustardBlack.Handlers.Binding;

namespace MustardBlack.Extensions
{
	public static class BinderCollectionExtensions
	{
		/// <summary>
		/// Inserts a binder after the first occurance of the searched (index) binder type.
		/// Throws if the searched type is not found
		/// </summary>
		/// <typeparam name="TIndex"></typeparam>
		/// <param name="binders"></param>
		/// <param name="binder"></param>
		public static void InsertAfter<TIndex>(this IList<IBinder> binders, IBinder binder)
		{
			for (var i = 0; i < binders.Count; i++)
			{
				if (binders[i].GetType().IsOrDerivesFrom<TIndex>())
				{
					binders.Insert(i+1, binder);
					return;
				}
			}

			throw new ArgumentException("Cannot locate binder of type `" + typeof(TIndex) + "`");
		}
	}
}
