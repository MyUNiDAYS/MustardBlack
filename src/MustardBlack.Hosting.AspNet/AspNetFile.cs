using System.IO;
using System.Web;

namespace MustardBlack.Hosting.AspNet
{
	sealed class AspNetFile : IFile
	{
		readonly HttpPostedFileBase file;

		public Stream InputStream => this.file.InputStream;
		public long ContentLength => this.file.ContentLength;
		public string ContentType => this.file.ContentType;
		public string FileName => this.file.FileName;
		
		public AspNetFile(HttpPostedFileBase file)
		{
			this.file = file;
		}
	}
}
