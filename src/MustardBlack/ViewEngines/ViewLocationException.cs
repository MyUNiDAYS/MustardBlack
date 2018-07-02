using System;
using System.Runtime.Serialization;

namespace MustardBlack.ViewEngines
{
	[Serializable]
	public sealed class ViewLocationException : Exception
	{
		public ViewLocationException()
		{
		}

		public ViewLocationException(string message) : base(message)
		{
		}

		public ViewLocationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		ViewLocationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}