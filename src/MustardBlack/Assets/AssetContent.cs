namespace MustardBlack.Assets
{
	public struct AssetContent
	{
		public string Path { get; }
		public string Contents { get; }

		public AssetContent(string path, string contents)
		{
			this.Path = path;
			this.Contents = contents;
		}
	}
}
