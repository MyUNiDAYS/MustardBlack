using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MustardBlack.Assets
{
	public interface IAssetLoader
	{
		string GetAsset(string path, Regex nameMatch);
		IEnumerable<Asset> GetAssets(string path, Regex nameMatch);
	}
}
