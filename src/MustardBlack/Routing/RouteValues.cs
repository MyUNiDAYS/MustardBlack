using System;
using System.Collections.Generic;

namespace MustardBlack.Routing
{
	public sealed class RouteValues : Dictionary<string, object>
	{
		public bool IsLocalised => this.ContainsKey("regioncode") && this.ContainsKey("culturecode");

		public RouteValues() : base(StringComparer.OrdinalIgnoreCase)
		{
		}

		public RouteValues(IDictionary<string, object> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase)
		{
		}

		public RouteValues(object values) : base(values.ToDictionary(), StringComparer.OrdinalIgnoreCase)
		{
		}

		public new object this[string key]
		{
			get
			{
				object v;
				return base.TryGetValue(key, out v) ? v : null;
			}
			set { base[key] = value; }
		}
	}
}