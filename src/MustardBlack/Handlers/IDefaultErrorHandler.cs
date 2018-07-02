using System;
using System.Net;
using MustardBlack.Hosting;
using MustardBlack.Results;

namespace MustardBlack.Handlers
{
	public interface IDefaultErrorHandler : IHandler
	{
		IResult NotFound(IRequest request, object data);
		IResult Error(IRequest request, Exception exception, object data, HttpStatusCode statusCode = HttpStatusCode.InternalServerError);
		IResult ServiceUnavailable(IRequest request, Exception exception, object data);
	}
}
