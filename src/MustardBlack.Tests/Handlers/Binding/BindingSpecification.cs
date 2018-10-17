using MustardBlack.Handlers.Binding;
using MustardBlack.Hosting;
using NanoIoC;
using NSubstitute;

namespace MustardBlack.Tests.Handlers.Binding
{
	public abstract class BindingSpecification : AutoMockedSpecification
	{
		protected IRequest Request;
		Container container;

		protected override void Given()
		{
			this.container = new Container();
			BinderCollection.Initialize(this.container);

			this.Request = Stub<IRequest>();
			
			this.Request.Headers.Returns(new HeaderCollection());
			this.Request.Form.Returns(new HeaderCollection());
		}

		public override void Dispose()
		{
			this.container.Reset();
		}
	}
}