using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Handlers.Binding;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;
using Serilog;

namespace MustardBlack.Handlers
{
	public sealed class HandlerExecutorPipelineOperator : IPreHandlerExecutionPipelineOperator
	{
		readonly IContainer container;
		readonly IRequestBinder requestBinder;
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public HandlerExecutorPipelineOperator(IContainer container, IRequestBinder requestBinder)
		{
			this.container = container;
			this.requestBinder = requestBinder;
		}

		public async Task<PipelineContinuation> Operate(PipelineContext context)
		{
			if (context.RouteData() == null)
				return PipelineContinuation.Continue;
			
			if (context.RouteData()?.Route == null)
			{
				context.Result = new ErrorResult(HttpStatusCode.NotFound);
				return PipelineContinuation.SkipToPostHandler;
			}

			// TODO: this doesnt really belong in here
			var handlerType = context.RouteData().Route.HandlerType;
			if (handlerType == null)
			{
				context.Result = new ErrorResult(HttpStatusCode.NotFound);
				return PipelineContinuation.SkipToPostHandler;
			}

			// TODO: this doesnt really belong in here
			var handlerAction = context.RouteData().HandlerAction;
			if (handlerAction == null)
			{
				context.Result = new ErrorResult(HttpStatusCode.MethodNotAllowed);
				return PipelineContinuation.SkipToPostHandler;
			}

			IHandler handler;

			try
			{
				handler = this.container.Resolve(handlerType) as IHandler;
				handler.Context = context;
				context.Items["Handler"] = handler;
			}
			catch (Exception e)
			{
				log.Error(e, "Failed to construct handler `{HandlerType}`", handlerType);
#if DEBUG
				if (Debugger.IsAttached) Debugger.Break();
#endif
				context.Result = ErrorResult.InternalServerError(e);
				return PipelineContinuation.SkipToPostHandler;
			}
			
			object[] parameters;
			try
			{
				parameters = this.requestBinder.GetAndValidateParameters(handler, handlerAction.HandleMethod, context.Request, context.RouteData().Values);
			}
			catch (Exception e)
			{
				log.Error(e, "Failed to bind/validate parameters for handler `{Handler}`", handler.GetType().FullName);
#if DEBUG
				if (Debugger.IsAttached) Debugger.Break();
#endif
				context.Result = ErrorResult.InternalServerError(e);
				return PipelineContinuation.SkipToPostHandler;
			}
			
			try
			{
				var handleMethod = handlerAction.HandleMethod;
			if (handleMethod.MethodReturnsTask())
					context.Result = await handleMethod.InvokeWithoutThrowingTargetInvocationException<Task<IResult>>(handler, parameters);
				else
					context.Result = handleMethod.InvokeWithoutThrowingTargetInvocationException<IResult>(handler, parameters);
			}
			catch (Exception e)
			{
				log.Error(e, "Failed to execute handler `{Handler}`", handler.GetType().FullName);
#if DEBUG
				if (Debugger.IsAttached) Debugger.Break();
#endif
				context.Result = ErrorResult.InternalServerError(e);
				return PipelineContinuation.SkipToPostHandler;
			}
			

			return PipelineContinuation.SkipToPostHandler;
		}
	}
}