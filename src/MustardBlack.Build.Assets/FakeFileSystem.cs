using System;
using System.Collections.Generic;
using System.IO;
using MustardBlack.Hosting;

namespace MustardBlack.Assets.Build
{
	sealed class FakeFileSystem : IFileSystem
	{
		public string GetFullPath(string path)
		{
			return path;
		}

		public bool Exists(string path)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> GetFiles(string folderPath)
		{
			throw new NotImplementedException();
		}

		public TResult Read<TResult>(string path, Func<StreamReader, TResult> streamAction)
		{
			throw new NotImplementedException();
		}

		public DateTime GetLastWriteTime(string path)
		{
			throw new NotImplementedException();
		}

		public void Write(Stream stream, string path)
		{
			throw new NotImplementedException();
		}
	}
}
