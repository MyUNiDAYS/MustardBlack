using System.IO;
using System.IO.Compression;
using System.Text;
using Brotli;
using MustardBlack.Brotli.NET;

namespace MustardBlack.Tests.Compression.Brotli
{
	public class MultipleWriting : Specification
	{
		Stream brotliStream;
		MemoryStream memoryStream;

		protected override void Given()
		{
			this.memoryStream = new MemoryStream();
			this.brotliStream = new BrotliStreamWrapper(this.memoryStream, () => { }, true);
		}

		protected override void When()
		{
			this.brotliStream.Write(Encoding.UTF8.GetBytes("one"), 0, 3);
			this.brotliStream.Write(Encoding.UTF8.GetBytes("two"), 0, 3);
			this.brotliStream.Flush();
			this.brotliStream.Write(Encoding.UTF8.GetBytes("three"), 0, 5);
			this.brotliStream.Flush();
			this.brotliStream.Dispose();
		}

		[Then]
		public void MemoryStreamShouldBeDecodable()
		{
			memoryStream.Position = 0;
			using (var bs = new BrotliStream(memoryStream, CompressionMode.Decompress))
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