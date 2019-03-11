using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		public string GetAsset(string path, Regex nameMatch)
		{
			var fullPath = this.fileSystem.GetFullPath(path.ToLowerInvariant());
			var filesContents = this.ReadFiles(fullPath, nameMatch);

			return filesContents;
		}

		public IEnumerable<AssetContent> GetAssets(string path, Regex nameMatch)
		{
			var fullPath = this.fileSystem.GetFullPath(path.ToLowerInvariant());
			var assets = ReadMultipleFilesIndividually(path, fullPath, nameMatch);

			return assets;
		}

		string ReadFiles(string fullPath, Regex nameMatch)
		{
			if (File.Exists(fullPath))
				return ReadSingleFile(fullPath);

			return this.ReadMultipleFiles(fullPath, nameMatch);
		}


		static IEnumerable<AssetContent> ReadMultipleFilesIndividually(string relativePath, string fullPath, Regex nameMatch)
		{
			var files = ListFiles(fullPath, nameMatch);
			var assets = new List<AssetContent>();
			foreach (var file in files)
			{
				var contents = ReadSingleFile(file);
				var path = relativePath.TrimStart('~') + file.Substring(fullPath.Length);
				assets.Add(new AssetContent(path, contents));
			}

			return assets;
		}

		string ReadMultipleFiles(string fullPath, Regex nameMatch)
		{
			var fileBuilder = new StringBuilder();

			var files = ListFiles(fullPath, nameMatch);
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

		static IEnumerable<string> ListFiles(string folder, Regex nameMatch, List<string> files = null)
		{
			if (files == null)
				files = new List<string>();

			if (!Directory.Exists(folder))
				return files;

			var directories = Directory.GetDirectories(folder).OrderBy(n => n);
			foreach (var dir in directories)
				ListFiles(dir, nameMatch, files);

			var dirFiles = Directory.GetFiles(folder)
				.Where(f => nameMatch.IsMatch(f))
				.OrderBy(n => n);
			files.AddRange(dirFiles);

			return files;
		}
	}
}
