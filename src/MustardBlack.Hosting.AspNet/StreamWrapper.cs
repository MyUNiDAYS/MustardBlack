using System;
using System.IO;

namespace MustardBlack.Hosting.AspNet
{
	sealed class StreamWrapper : Stream
	{
		readonly Stream baseStream;
		readonly Action writeAction;
		bool written;

		public StreamWrapper(Stream baseStream, Action writeAction)
		{
			this.baseStream = baseStream;
			this.writeAction = writeAction;
		}

		public override void Flush()
		{
			this.baseStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.baseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this.baseStream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.baseStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.baseStream.Write(buffer, offset, count);

			if (!this.written && count > 0)
			{
				this.written = true;
				this.writeAction();
			}
		}

		public override bool CanRead => this.baseStream.CanRead;
		public override bool CanSeek => this.baseStream.CanSeek;
		public override bool CanWrite => this.baseStream.CanWrite;
		public override long Length => this.baseStream.Length;
		public override long Position
		{
			get => this.baseStream.Position;
			set => this.baseStream.Position = value;
		}
	}
}