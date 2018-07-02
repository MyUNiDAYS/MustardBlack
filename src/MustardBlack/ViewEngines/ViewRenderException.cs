using System;
using System.Runtime.Serialization;

namespace MustardBlack.ViewEngines
{
	[Serializable]
	public sealed class ViewRenderException : Exception
	{
		public ViewRenderException()
		{
		}

		public ViewRenderException(string message) : base(message)
		{
		}

		public ViewRenderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		ViewRenderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}