using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Brotli;

namespace MustardBlack.Brotli.NET
{
	public sealed class BrotliStreamWrapper : BrotliStream
	{
		readonly Action writeAction;
		bool written;

		public BrotliStreamWrapper(Stream baseStream, Action writeAction, bool leaveOpen = false) : base(baseStream, CompressionMode.Compress, leaveOpen)
		{
			this.writeAction = writeAction;
		}

		protected override void FlushBrotliStream(bool finished)
		{
			if (!(this._state == IntPtr.Zero) && !Brolib.BrotliEncoderIsFinished(this._state))
			{
				BrotliEncoderOperation op = finished ? BrotliEncoderOperation.Finish : BrotliEncoderOperation.Flush;
				uint totalOut = 0;
				while (Brolib.BrotliEncoderCompressStream(this._state, op, ref this._availableIn, ref this._ptrNextInput, ref this._availableOut, ref this._ptrNextOutput, out totalOut))
				{
					bool flag = this._availableOut != 65536U;
					if (flag)
					{
						int num = 65536 - (int) this._availableOut;
						Marshal.Copy(this._ptrOutputBuffer, this._managedBuffer, 0, num);
						if(this.written)
							this._stream.Write(this._managedBuffer, 0, num);
						this._availableOut = 65536U;
						this._ptrNextOutput = this._ptrOutputBuffer;
					}

					if (Brolib.BrotliEncoderIsFinished(this._state) || !flag)
						return;
				}

				throw new BrotliException("Unable to finish encode stream");
			}
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