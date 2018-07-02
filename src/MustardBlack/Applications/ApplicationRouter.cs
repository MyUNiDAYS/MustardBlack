using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using Serilog;

namespace MustardBlack.Applications
{
    public sealed class ApplicationRouter : IApplicationRouter
    {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		readonly IEnumerable<IApplication> applications;

        public ApplicationRouter(IEnumerable<IApplication> applications)
        {
            this.applications = applications;
        }

        public async Task RouteApplication(PipelineContext context)
        {
	        log.Debug("Routing request to an application");

			foreach (var app in this.applications)
            {
				if (app.CanServe(context.Request))
	            {
					log.Debug("Request successfully routed to {application}.", app.GetType());

					await app.Serve(context);
		            return;
	            }

	            log.Debug("Request not routed to {application}", app.GetType());
			}

	        log.Warning("Could not identify any application to serve the request");
			context.Response.StatusCode = HttpStatusCode.NotImplemented;
        }
    }
}
