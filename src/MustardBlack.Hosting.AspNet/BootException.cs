using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace MustardBlack.Hosting.AspNet
{
	public class BootException : Exception
	{
		public BootException(IEnumerable<Exception> exceptions, Exception innerException) :
			base("Unable to boot application\n" + string.Join("\n", exceptions.Select(e =>
			{
				var message = e.Message;

				if(e is FileNotFoundException fnfe)
					message += "\n" + fnfe.FusionLog;

				if(e is FileLoadException fle)
					message += "\n" + fle.FusionLog;
				return message;
			})), innerException)
		{
		}

		protected BootException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
