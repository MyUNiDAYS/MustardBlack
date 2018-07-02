namespace MustardBlack.Assets
{
	public interface IAssetLoader
	{
		string GetAsset(string path, AssetFormat format);
	}
}
