using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MustardBlack.Extensions
{
	public static class ExceptionExtensions
	{
		public static Exception CreateDetailedException(this TypeInitializationException exception)
		{
			if (exception.InnerException is ReflectionTypeLoadException inner)
				return inner.CreateDetailedException();

			return exception;
		}

		public static Exception CreateDetailedException(this ReflectionTypeLoadException exception)
		{
			return new TypeLoadException("Unable to load one or more types:\n" + GetExceptionMessage(exception.LoaderExceptions), exception);
		}

		static string GetExceptionMessage(IEnumerable<Exception> exceptions)
		{
			return string.Join("\n", exceptions.Select(e =>
			{
				var message = e.Message;

				if (e is FileNotFoundException fnfe)
					message += "\n" + fnfe.FusionLog;

				if (e is FileLoadException fle)
					message += "\n" + fle.FusionLog;
				return message;
			}));
		}
	}
}