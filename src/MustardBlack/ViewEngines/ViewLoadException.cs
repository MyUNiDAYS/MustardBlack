using System;
using System.Runtime.Serialization;

namespace MustardBlack.ViewEngines
{
	[Serializable]
	public sealed class ViewLoadException : Exception
	{
		public ViewLoadException()
		{
		}

		public ViewLoadException(string message) : base(message)
		{
		}

		public ViewLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}

		ViewLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}