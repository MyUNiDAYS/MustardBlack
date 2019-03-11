namespace MustardBlack.Assets
{
	public struct AssetContent
	{
		public string FullPath { get; }
		public string Contents { get; }

		public AssetContent(string fullPath, string contents)
		{
			this.FullPath = fullPath;
			this.Contents = contents;
		}
	}
}
