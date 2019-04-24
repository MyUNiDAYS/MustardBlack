using System;

namespace MustardBlack.Assets
{
	public interface IAssetCache
	{
		TResult GetAsset<TResult>(string key, Func<DateTime> getLastModified, Func<TResult> getContents) where TResult : class;
	}
}