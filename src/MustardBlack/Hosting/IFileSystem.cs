using System;
using System.Collections.Generic;
using System.IO;

namespace MustardBlack.Hosting
{
	public interface IFileSystem
	{
		/// <summary>
		/// Turns a virtual path into a full path on disc
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		string GetFullPath(string path);

		/// <summary>
		/// Determines if a path exists
		/// </summary>
		/// <param name="path">Virtual or absolute path</param>
		/// <returns></returns>
		bool Exists(string path);

		/// <summary>
		/// Lists the full path of all the files within the given folder path
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		IEnumerable<string> GetFiles(string folderPath);

		TResult Read<TResult>(string path, Func<StreamReader, TResult> streamAction);

		/// <summary>
		/// Gets the last modified datetime of a virtual or absolute file path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		DateTime GetLastWriteTime(string path);

		/// <summary>
		/// Writes the given stream to the given path
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="path"></param>
		void Write(Stream stream, string path);
	}
}
