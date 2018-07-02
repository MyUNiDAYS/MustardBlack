using System;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class CachedItem
	{
		public readonly Type ViewType;
		public readonly DateTime LastModified;

		public CachedItem(Type viewType, DateTime lastModified)
		{
			this.ViewType = viewType;
			this.LastModified = lastModified;
		}
	}
}