namespace MustardBlack.Results
{
	public sealed class FilePathResult : FileResult
	{
		public readonly string Path;

		/// <summary>
		/// Creates a new File Content Result which will appear to the user as a "Save File" popup, rather than displaying in-browser.
		/// </summary>
		/// <param name="filename">The name the file will be downloaded as</param>
		/// <param name="contentType">the content type</param>
		/// <param name="path">the path to the file</param>
		public FilePathResult(string filename, string contentType, string path) : base(filename, contentType)
		{
			this.Path = path;
		}

		/// <summary>
		/// Creates a new File Content Result which will appear to the user in-browser where possible, rather than a "Save File" popup.
		/// </summary>
		/// <param name="contentType">the content type</param>
		/// <param name="path">the path to the file</param>
		public FilePathResult(string contentType, string path) : base(contentType)
		{
			this.Path = path;
		}
	}
}