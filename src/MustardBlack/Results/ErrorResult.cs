using System;
using System.Net;

namespace MustardBlack.Results
{
	public sealed class ErrorResult : Result
	{
		public readonly Exception Exception;
		public readonly object Data;

		public ErrorResult(HttpStatusCode statusCode, Exception exception = null, object data = null)
		{
			this.StatusCode = statusCode;
			this.Exception = exception;
			this.Data = data;
		}
		
		public static ErrorResult NotFound(object data = null)
		{
			return new ErrorResult(HttpStatusCode.NotFound, null, data);
		}

		public static ErrorResult Conflict(object data = null)
		{
			return new ErrorResult(HttpStatusCode.Conflict, null, data);
		}

		public static ErrorResult ServiceUnavailable(object data = null)
		{
			return new ErrorResult(HttpStatusCode.ServiceUnavailable, null, data = null);
		}

		public static ErrorResult BadRequest(object data = null)
		{
			return new ErrorResult(HttpStatusCode.BadRequest, null, data);
		}

		public static ErrorResult InternalServerError(Exception e = null, object data = null)
		{
			return new ErrorResult(HttpStatusCode.InternalServerError, e, data);
		}

		public static ErrorResult Unauthorized(object data = null)
		{
			return new ErrorResult(HttpStatusCode.Unauthorized, null , data);
		}

		public static ErrorResult Forbidden(object data = null)
		{
			return new ErrorResult(HttpStatusCode.Forbidden, null , data);
		}
	}
}
