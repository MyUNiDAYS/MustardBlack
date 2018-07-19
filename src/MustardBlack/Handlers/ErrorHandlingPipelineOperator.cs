using System;
using System.Net;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;

namespace MustardBlack.Handlers
{
	public sealed class ErrorHandlingPipelineOperator : IPostResultPipelineOperator
	{
		readonly IContainer container;

		public ErrorHandlingPipelineOperator(IContainer container)
		{
			this.container = container;
		}

		public Task<PipelineContinuation> Operate(PipelineContext context)
		{
			if (context.Result is ErrorResult)
			{
				var errorResult = context.Result as ErrorResult;
				this.HandleError(context, errorResult.Exception, errorResult.Data, errorResult.StatusCode);
			}

			return Task.FromResult(PipelineContinuation.Continue);
		}

		void HandleError(PipelineContext context, Exception exception, object data, HttpStatusCode statusCode)
		{
			if (context.Application.DefaultErrorHandler == null)
			{
				context.Result = new EmptyResult(statusCode);
				return;
			}

			var errorHandler = this.container.Resolve(context.Application.DefaultErrorHandler) as IDefaultErrorHandler;
			errorHandler.Context = context;

			if (statusCode == HttpStatusCode.NotFound)
				context.Result = errorHandler.NotFound(context.Request, data);
			else if (statusCode == HttpStatusCode.ServiceUnavailable)
				context.Result = errorHandler.ServiceUnavailable(context.Request, exception, data);
			// TODO: handle 405 here?
			else
				context.Result = errorHandler.Error(context.Request, exception, data, statusCode);
		}
	}
}
