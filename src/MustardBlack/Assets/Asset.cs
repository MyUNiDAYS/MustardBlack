namespace MustardBlack.Assets
{
	public struct Asset
	{
		public string FullPath { get; }
		public string Contents { get; }

		public Asset(string fullPath, string contents)
		{
			this.FullPath = fullPath;
			this.Contents = contents;
		}
	}
}
