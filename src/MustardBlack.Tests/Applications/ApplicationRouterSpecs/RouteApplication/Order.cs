using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MustardBlack.Applications;
using MustardBlack.Authentication;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using MustardBlack.Routing;
using NSubstitute;

namespace MustardBlack.Tests.Applications.ApplicationRouterSpecs
{
	public class Order : Specification
	{
		TestApplication[] applications;
		private ApplicationRouter subject;
		private PipelineContext pipelineContext;

		protected override void Given()
		{
			this.applications = new[]
			{
				new TestApplication(1, false),
				new TestApplication(2, true),
				new TestApplication(3, true)
			};

			subject = new ApplicationRouter(this.applications);

			pipelineContext = new PipelineContext(Substitute.For<IRequest>(), Substitute.For<IResponse>());
		}

		protected override void When()
		{
			subject.RouteApplication(pipelineContext).Wait();
		}

		[Then]
		public void TheFirstServingAppOnlyShouldServe()
		{
			this.pipelineContext.Items.ContainsKey("1").Should().BeFalse();
			this.pipelineContext.Items.ContainsKey("2").Should().BeTrue();
			this.pipelineContext.Items.ContainsKey("3").Should().BeFalse();
		}

		sealed class TestApplication : IApplication
		{
			readonly int appId;
			readonly bool canServe;

			public TestApplication(int appId, bool canServe)
			{
				this.appId = appId;
				this.canServe = canServe;
			}

			public bool CanServe(IRequest request)
			{
				return this.canServe;
			}

			public IEnumerable<IAuthenticationMechanism> AuthenticationMechanisms { get; }
			public ICollection<IRoute> Routes { get; }
			public IHandlerCache HandlerCache { get; }
			public Type DefaultErrorHandler { get; }
			public Task Serve(PipelineContext context)
			{
				context.Items.Add(this.appId.ToString(), true);
				return Task.CompletedTask;
			}

			public RouteData RouteRequest(Url url, HttpMethod method, RequestType requestType)
			{
				throw new NotImplementedException();
			}
		}
	}
}
