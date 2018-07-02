using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MustardBlack.Authentication;
using MustardBlack.Handlers;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;
using MustardBlack.Routing;

namespace MustardBlack.Applications
{
    public interface IApplication
    {
		/// <summary>
		/// Determines if this Application can serve the given request
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
        bool CanServe(IRequest request);

		/// <summary>
		/// Returns a list of supported IAuthenticationMechanisms for this Application
		/// </summary>
        IEnumerable<IAuthenticationMechanism> AuthenticationMechanisms { get; }

		/// <summary>
		/// Returns a list of IRoutes configured for this Application
		/// </summary>
	    ICollection<IRoute> Routes { get; }
		
        IHandlerCache HandlerCache { get; }
		Type DefaultErrorHandler { get; }

		/// <summary>
		/// Serves the given request
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
	    Task Serve(PipelineContext context);

	    RouteData RouteRequest(Url url, HttpMethod method, RequestType requestType);
    }
}
