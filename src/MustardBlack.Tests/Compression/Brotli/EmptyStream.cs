using System.IO;
using FluentAssertions;
using MustardBlack.Hosting.AspNet;
using NUnit.Framework;

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
				Assert.Fail("Should not execute");
			}, true);
		}

		protected override void When()
		{
			this.brotliStream.Flush();
		}

		[Then]
		public void MemoryStreamShouldBeEmpty()
		{
			this.memoryStream.Length.Should().Be(0);
		}
	}
}