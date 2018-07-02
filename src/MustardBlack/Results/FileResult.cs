using System;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace MustardBlack.Results
{
	public abstract class FileResult : Result
	{
		public readonly string ContentDisposition;
		public readonly string ContentType;

		protected FileResult(string contentType, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			this.ContentType = contentType;
			this.StatusCode = statusCode;
		}

		protected FileResult(string filename, string contentType, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (!string.IsNullOrEmpty(filename))
				this.ContentDisposition = GetHeaderValue(filename);

			this.ContentType = contentType;
			this.StatusCode = statusCode;
		}

		public static string GetHeaderValue(string fileName)
		{
			try
			{
				var cd = new ContentDisposition
				         	{
				         		FileName = fileName
				         	};
				return cd.ToString();
			}
			catch (FormatException)
			{
				return CreateRfc2231HeaderValue(fileName);
			}
		}

		static string CreateRfc2231HeaderValue(string filename)
		{
			var builder = new StringBuilder("attachment; filename*=UTF-8''");
			foreach (byte b in Encoding.UTF8.GetBytes(filename))
			{
				if (IsByteValidHeaderValueCharacter(b))
					builder.Append((char)b);
				else
					AddByteToStringBuilder(b, builder);
			}
			return builder.ToString();
		}

		static void AddByteToStringBuilder(byte b, StringBuilder builder)
		{
			builder.Append('%');
			int i = b;
			AddHexDigitToStringBuilder(i >> 4, builder);
			AddHexDigitToStringBuilder(i % 0x10, builder);
		}

		static void AddHexDigitToStringBuilder(int digit, StringBuilder builder)
		{
			builder.Append("0123456789ABCDEF"[digit]);
		}

		static bool IsByteValidHeaderValueCharacter(byte b)
		{
			if ((0x30 <= b) && (b <= 0x39))
				return true;

			if ((0x61 <= b) && (b <= 0x7a))
				return true;

			if ((0x41 <= b) && (b <= 90))
				return true;

			switch (b)
			{
				case 0x3a:
				case 0x5f:
				case 0x7e:
				case 0x24:
				case 0x26:
				case 0x21:
				case 0x2b:
				case 0x2d:
				case 0x2e:
					return true;
			}

			return false;
		}
	}
}
