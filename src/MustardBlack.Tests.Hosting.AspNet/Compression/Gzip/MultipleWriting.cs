using System.IO;
using System.IO.Compression;
using System.Text;
using MustardBlack.Hosting.AspNet;

namespace MustardBlack.Tests.Hosting.AspNet.Compression.Gzip
{
	public class MultipleWriting : Specification
	{
		Stream gzipStream;
		MemoryStream memoryStream;

		protected override void Given()
		{
			this.memoryStream = new MemoryStream();
			this.gzipStream = new GzipStreamWrapper(this.memoryStream, () => { }, true);
		}

		protected override void When()
		{
			this.gzipStream.Write(Encoding.UTF8.GetBytes("one"), 0, 3);
			this.gzipStream.Write(Encoding.UTF8.GetBytes("two"), 0, 3);
			this.gzipStream.Flush();
			this.gzipStream.Write(Encoding.UTF8.GetBytes("three"), 0, 5);
			this.gzipStream.Flush();
			this.gzipStream.Dispose();
		}

		[Then]
		public void MemoryStreamShouldBeDecodable()
		{
			this.memoryStream.Position = 0;
			using (var bs = new GZipStream(this.memoryStream, CompressionMode.Decompress))
			using (var msOutput = new MemoryStream())
			{
				bs.CopyTo(msOutput);
				msOutput.Seek(0, SeekOrigin.Begin);
				var output = msOutput.ToArray();
				Encoding.UTF8.GetString(output).ShouldEqual("onetwothree");
			}
		}
	}
}