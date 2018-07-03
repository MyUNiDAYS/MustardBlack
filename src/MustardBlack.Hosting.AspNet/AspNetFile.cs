using System.IO;
using System.Web;

namespace MustardBlack.Hosting.AspNet
{
	// Extending HttpPostedFileBase here is a backwards compatability/developer usability convenience. 
	// Really it wouldnt do this
	sealed class AspNetFile : HttpPostedFileBase, IFile
	{
		readonly HttpPostedFileBase file;

		public override Stream InputStream => this.file.InputStream;
		public override int ContentLength => this.file.ContentLength;
		public override string ContentType => this.file.ContentType;
		public override string FileName => this.file.FileName;

		public AspNetFile(HttpPostedFileBase file)
		{
			this.file = file;
		}

		public override void SaveAs(string filename)
		{
			file.SaveAs(filename);
		}
	}
}
