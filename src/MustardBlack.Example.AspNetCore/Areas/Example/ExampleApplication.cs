using System;
using MustardBlack.Applications;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Results;
using MustardBlack.Routing;
using NanoIoC;

namespace MustardBlack.Example.AspNetCore.Areas.Example
{
	sealed class ExampleApplication : ApplicationBase
	{
		public ExampleApplication(IContainer container) : base(container)
		{
		}

		public override Type DefaultErrorHandler => null;

		public override bool CanServe(IRequest request)
		{
			return true;
		}
		
		protected override void Configure()
		{
			this.RegisterArea<ExampleAreaRegistration>();
			
			this.RegisterPipelineOperator<RoutingPipelineOperator>();
			this.RegisterPipelineOperator<HandlerExecutorPipelineOperator>();
			this.RegisterPipelineOperator<ErrorHandlingPipelineOperator>();
			this.RegisterPipelineOperator<ResultExecutorPipelineOperator>();
		}
	}
}
