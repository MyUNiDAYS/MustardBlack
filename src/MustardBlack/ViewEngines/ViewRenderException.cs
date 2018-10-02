using System;
using System.Runtime.Serialization;

namespace MustardBlack.ViewEngines
{
	[Serializable]
	public sealed class ViewRenderException : Exception
	{
	    public readonly string ViewSource;
	    public readonly string GeneratedCSharp;

	    public ViewRenderException(string message) : base(message)
		{
		}

	    public ViewRenderException(string message, string viewSource, string generatedCSharp)  :base(message)
	    {
	        this.ViewSource = viewSource;
	        this.GeneratedCSharp = generatedCSharp;
	    }

	    public ViewRenderException(string message, Exception innerException) : base(message, innerException)
		{
		}

	    ViewRenderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}