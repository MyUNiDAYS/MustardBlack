using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MustardBlack.Hosting;

namespace MustardBlack.Assets
{
	public sealed class AssetLoader : IAssetLoader
	{
		readonly IFileSystem fileSystem;

		public AssetLoader(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		public string GetAsset(string path, AssetFormat format)
		{
			var fullPath = this.fileSystem.GetFullPath(path.ToLowerInvariant());
			var filesContents = this.ReadFiles(fullPath, format);

			return filesContents;
		}

		string ReadFiles(string fullPath, AssetFormat format)
		{
			if (File.Exists(fullPath))
				return ReadSingleFile(fullPath);

			return this.ReadMultipleFiles(fullPath, format);
		}

		string ReadMultipleFiles(string fullPath, AssetFormat format)
		{
			var fileBuilder = new StringBuilder();

			var files = ListFiles(fullPath, format);
			foreach (var file in files)
			{
				var singleFile = ReadSingleFile(file);
				fileBuilder.AppendLine(singleFile);
			}
			
			return fileBuilder.ToString();
		}

		static string ReadSingleFile(string fullPath)
		{
			using (var streamReader = new StreamReader(fullPath, true))
				return streamReader.ReadToEnd();
		}

		static IEnumerable<string> ListFiles(string folder, AssetFormat format, List<string> files = null)
		{
			if (files == null)
				files = new List<string>();

			if (!Directory.Exists(folder))
				return files;

			var directories = Directory.GetDirectories(folder).OrderBy(n => n);
			foreach (var dir in directories)
				ListFiles(dir, format, files);

			var dirFiles = Directory.GetFiles(folder).Where(f =>
			{
				if (format == AssetFormat.Js)
                    return (!f.EndsWith("test.js") && f.EndsWith(".js"));

				if (format == AssetFormat.Css)
					return f.EndsWith(".css") || f.EndsWith(".less");

				return false;
			}).OrderBy(n => n);
			files.AddRange(dirFiles);

			return files;
		}
	}
}
