using System.Net;

namespace MustardBlack.Results
{
	public sealed class FileContentResult : FileResult
	{
		public readonly byte[] Data;

		/// <summary>
		/// Creates a new File Content Result which will appear to the user as a "Save File" popup, rather than displaying in-browser.
		/// </summary>
		/// <param name="filename">The default "save as" name of the file.</param>
		/// <param name="contentType">The media type for the file</param>
		/// <param name="data">The file data</param>
		/// <param name="statusCode"></param>
		public FileContentResult(string filename, string contentType, byte[] data, HttpStatusCode statusCode = HttpStatusCode.OK) : base(filename, contentType, statusCode)
		{
			this.Data = data;
		}

		/// <summary>
		/// Creates a new File Content Result which will appear to the user in-browser where possible, rather than a "Save File" popup.
		/// </summary>
		/// <param name="contentType">The media type for the file</param>
		/// <param name="data">The file data</param>
		/// <param name="statusCode"></param>
		public FileContentResult(string contentType, byte[] data, HttpStatusCode statusCode = HttpStatusCode.OK) : base(contentType, statusCode)
		{
			this.Data = data;
		}
	}
}
