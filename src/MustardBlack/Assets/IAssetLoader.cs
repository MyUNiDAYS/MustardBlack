using System.Text.RegularExpressions;

namespace MustardBlack.Assets
{
	public interface IAssetLoader
	{
		string GetAsset(string path, Regex nameMatch);
	}
}
