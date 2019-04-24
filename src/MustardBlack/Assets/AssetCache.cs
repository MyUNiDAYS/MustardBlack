using System;
using System.Collections.Generic;

namespace MustardBlack.Assets
{
	sealed class AssetCache : IAssetCache
	{
		readonly IDictionary<string, CachedItem> cache;

		public AssetCache()
		{
			this.cache = new Dictionary<string, CachedItem>();
		}

		public TResult GetAsset<TResult>(string key, Func<DateTime> getLastModified, Func<TResult> getContents) where TResult : class
		{
			var lastModified = getLastModified();

			lock (this.cache)
			{
				if (this.cache.ContainsKey(key))
				{
					if (this.cache[key].LastModified == lastModified)
						return this.cache[key].Contents as TResult;

					this.cache.Remove(key);
				}

				this.cache[key] = new CachedItem
				{
					LastModified = lastModified,
					Contents = getContents()
				};

				return this.cache[key].Contents as TResult;
			}
		}

		sealed class CachedItem
		{
			public DateTime LastModified { get; set; }
			public object Contents { get; set; }
		}
	}
}