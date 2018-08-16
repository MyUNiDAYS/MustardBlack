using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MustardBlack.Handlers;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;

namespace MustardBlack.Tests.Handlers.HandlerCacheSpecs
{
	public class ShouldRegisterAllAttributePOs : Specification
	{
		private HandlerCache handlerCache;

		[DerivedAttribute]
		class TestHandler : Handler
		{
			public IResult Get()
			{
				return null;
			}
		}

		class BaseAttribute : Attribute
		{
		}

		class DerivedAttribute : BaseAttribute
		{

		}

		class BaseAttributePO : IAttributePipelineOperator<BaseAttribute>
		{
			public Task<PipelineContinuation> Operate(PipelineContext context)
			{
				throw new NotImplementedException();
			}

			public int Order { get; }
			public object Attribute { get; set; }
		}
		class DerivedAttributePO : IAttributePipelineOperator<DerivedAttribute>
		{
			public Task<PipelineContinuation> Operate(PipelineContext context)
			{
				throw new NotImplementedException();
			}

			public int Order { get; }
			public object Attribute { get; set; }
		}

		protected override void Given()
		{
			var container = new Container();
			container.Register<IAttributePipelineOperator<BaseAttribute>, BaseAttributePO>();
			container.Register<IAttributePipelineOperator<DerivedAttribute>, DerivedAttributePO>();
			this.handlerCache = new HandlerCache(container);
		}

		protected override void When()
		{
			this.handlerCache.Warm(new[] { typeof(TestHandler) });
		}

		[Then]
		public void BothOperatorsShouldBeReturnedInOrder()
		{
			var handlerAction = this.handlerCache.GetHandlerAction(typeof(TestHandler), HttpMethod.Get);

			handlerAction.Operators.Any(op => op.OperatorType == typeof(IAttributePipelineOperator<BaseAttribute>)).Should().BeTrue();
			handlerAction.Operators.Any(op => op.OperatorType == typeof(IAttributePipelineOperator<DerivedAttribute>)).Should().BeTrue();
		}
	}
}
