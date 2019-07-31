using System.IO;
using Microsoft.AspNetCore.Http;

namespace MustardBlack.Hosting.AspNetCore
{
	public sealed class AspNetCoreFile : IFile
	{
		readonly IFormFile file;
		public Stream InputStream => this.file.OpenReadStream();
		public long ContentLength => this.file.Length;
		public string ContentType => this.file.ContentType;
		public string FileName => this.file.FileName;

		public AspNetCoreFile(IFormFile file)
		{
			this.file = file;
		}
	}
}