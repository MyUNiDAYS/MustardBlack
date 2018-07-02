using System;
using System.IO;

namespace MustardBlack.Results
{
	public sealed class FileStreamResult : FileResult
	{
		public readonly Stream FileStream;

		/// <summary>
		/// Creates a new File Content Result which will appear to the user as a "Save File" popup, rather than displaying in-browser.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="contentType"></param>
		/// <param name="fileStream"></param>
		public FileStreamResult(string filename, string contentType, Stream fileStream)
			: base(filename, contentType)
		{
			if(fileStream == null)
				throw new ArgumentException("Stream cannot be null", "fileStream");

			this.FileStream = fileStream;
		}

		/// <summary>
		/// Creates a new File Content Result which will appear to the user in-browser where possible, rather than a "Save File" popup.
		/// </summary>
		/// <param name="contentType"></param>
		/// <param name="fileStream"></param>
		public FileStreamResult(string contentType, Stream fileStream) : base(contentType)
		{
			if (fileStream == null)
				throw new ArgumentException("Stream cannot be null", "fileStream");

			this.FileStream = fileStream;
		}
	}
}