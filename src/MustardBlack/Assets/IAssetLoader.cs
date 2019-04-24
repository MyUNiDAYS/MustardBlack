using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MustardBlack.Assets
{
	public interface IAssetLoader
	{
		string GetAsset(string path, Regex nameMatch);
		IEnumerable<AssetContent> GetAssets(string path, Regex nameMatch);
		DateTime GetMaxLastModified(string path, Regex nameMatch);
	}
}
