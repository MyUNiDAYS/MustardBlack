using System.IO;

namespace MustardBlack.Hosting.AspNetCore
{
	// Extending HttpPostedFileBase here is a backwards compatability/developer usability convenience. 
	// Really it wouldnt do this
	sealed class AspNetFile : IFile
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
			this.file.SaveAs(filename);
		}
	}
}
