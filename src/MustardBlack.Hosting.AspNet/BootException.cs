using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MustardBlack.Hosting.AspNet
{
	public class BootException : Exception
	{
		public BootException(IEnumerable<Exception> exceptions, Exception innerException) : base("Unable to boot applicaiton\n" + string.Join("\n", exceptions.Select(e => e.Message)), innerException)
		{
		}

		protected BootException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
