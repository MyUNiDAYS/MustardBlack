using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Areas;
using MustardBlack.Authentication;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using MustardBlack.Routing;
using NanoIoC;
using Serilog;

namespace MustardBlack.Applications
{
	public abstract class ApplicationBase : IApplication
	{
		protected static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		protected readonly IContainer container;
		public IEnumerable<IAuthenticationMechanism> AuthenticationMechanisms => this.authenticationMechanisms;
		public ICollection<IRoute> Routes { get; protected set; }
		public IHandlerCache HandlerCache { get; protected set; }
		public abstract Type DefaultErrorHandler { get; }

		readonly List<IAuthenticationMechanism> authenticationMechanisms;

		readonly List<Type> pipelineOperators;

		protected ApplicationBase(IContainer container)
		{
			this.container = container;
			this.pipelineOperators = new List<Type>();
			this.authenticationMechanisms = new List<IAuthenticationMechanism>();
			this.HandlerCache = this.container.Resolve<IHandlerCache>();
			this.Routes = new List<IRoute>();

			log.Debug("Configuring Application {Application}", this.GetType());
			this.Configure();
		}

		public abstract bool CanServe(IRequest request);

		public virtual Task Serve(PipelineContext context)
		{
			context.Application = this;
			var operatorInstances = new List<IPipelineOperator>();
			foreach (var pipelineOperatorType in this.pipelineOperators)
				operatorInstances.Add(this.container.Resolve(pipelineOperatorType) as IPipelineOperator);

			return PipelinePumper.Pump(context, operatorInstances);
		}

		public RouteData RouteRequest(Url url, HttpMethod method, RequestType requestType)
		{
			return this.Routes
				.Select(r => new RouteData(r, r.GetRouteValues(url, method, requestType)))
				.FirstOrDefault(x => x.Values != null);
		}
		
		protected void RegisterPipelineOperator<T>() where T : IPipelineOperator
		{
			log.Debug("Registering PipelineOperator {PipelineOperator} in position {position}", typeof(T), this.pipelineOperators.Count);
			this.pipelineOperators.Add(typeof(T));
		}

		protected void RegisterAuthenticationMechanism<T>() where T : IAuthenticationMechanism
		{
			log.Debug("Registering Authentication Mechanism {AuthenticationMechanism} in position {position}", typeof(T), this.authenticationMechanisms.Count);
			var authMechanism = this.container.Resolve<T>();
			this.authenticationMechanisms.Add(authMechanism);
		}

		protected void RegisterArea<T>() where T : AreaRegistrationBase
		{
			log.Debug("Registering Area {Area}", typeof(T));
			this.container.Resolve<T>().RegisterArea(this.HandlerCache, this.Routes);
		}

		protected abstract void Configure();
	}
}
