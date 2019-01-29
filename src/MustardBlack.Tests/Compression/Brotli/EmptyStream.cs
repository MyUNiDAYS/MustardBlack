using System.IO;
using MustardBlack.Compression;
using Xunit;

namespace MustardBlack.Tests.Compression.Brotli
{
	public class EmptyStream : Specification
	{
		Stream brotliStream;
		MemoryStream memoryStream;

		protected override void Given()
		{
			this.memoryStream = new MemoryStream();
			this.brotliStream = new BrotliStreamWrapper(this.memoryStream, () =>
			{
				Assert.False(true, "Should not execute");
			}, true);
		}

		protected override void When()
		{
			this.brotliStream.Flush();
		}

		[Then]
		public void MemoryStreamShouldBeEmpty()
		{
			this.memoryStream.Length.ShouldEqual(0);
		}
	}
}