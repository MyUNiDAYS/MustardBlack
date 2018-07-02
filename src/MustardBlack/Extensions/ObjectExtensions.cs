using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	static class ObjectExtensions
	{
		public static IDictionary<string, object> ToDictionary(this object data, IEqualityComparer<string> equalityComparer = null)
		{
			const BindingFlags attr = BindingFlags.Public | BindingFlags.Instance;

			var dict = new Dictionary<string, object>(equalityComparer);

			var properties = data.GetType().GetProperties(attr).Where(property => property.CanRead);
			foreach (var property in properties)
			{
				try
				{
					dict.Add(property.Name, property.GetValue(data, null));
				}
				catch (Exception e)
				{
					dict.Add(property.Name, "Error getting value: " + e.Message);
				}
			}

			return dict;
		}
	}
}