using System;
using System.IO;
using System.IO.Compression;
using Brotli;

namespace MustardBlack.Hosting.AspNet
{
	sealed class BrotliStreamWrapper : BrotliStream
	{
		readonly Action writeAction;
		bool written;

		public BrotliStreamWrapper(Stream baseStream, Action writeAction, bool leaveOpen = false) : base(baseStream, CompressionMode.Compress, leaveOpen)
		{
			this.writeAction = writeAction;
		}
		public override void Flush()
		{
			// Because Brotli seems buggy and will return ~2 bytes for a compressed empty stream
			if (written)
				base.Flush();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			base.Write(buffer, offset, count);

			if (!this.written)
			{
				this.written = true;
				this.writeAction();
			}
		}
	}
}