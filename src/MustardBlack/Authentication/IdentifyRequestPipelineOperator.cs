using System;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using Serilog;

namespace MustardBlack.Authentication
{
	public sealed class IdentifyRequestPipelineOperator : IPreResultPipelineOperator
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		public async Task<PipelineContinuation> Operate(PipelineContext context)
		{
			var authenticationMechanisms = context.Application.AuthenticationMechanisms;

			IAuthTicket authTicket = null;
			IAuthenticationMechanism authenticationMechanism = null;

			foreach (var authMechanism in authenticationMechanisms)
			{
				authenticationMechanism = authMechanism;
				log.Debug("Trying to identify request using {authenticationMethod}", authMechanism);

				authTicket = await authMechanism.TryIdentify(context);
				if (authTicket != null)
				{
					log.Debug("Request successfully identified using {authenticationMethod}", authMechanism);


					context.SetAuthTicket(authTicket);
					context.Items["__OriginalAuthTicket"] = authTicket;
					context.Items["__AuthenticationMethod"] = authMechanism;
					break;
				}
			}

			if (context.Result != null)
			{
				log.Debug("{authenticationMethod} returned identification challenge, returning", authenticationMechanism.GetType());
				return PipelineContinuation.SkipToPostHandler;
			}

			if (authTicket == null)
			{
				log.Debug("Request unidentified");

				authTicket = new AuthTicket();
				context.SetAuthTicket(authTicket);
				context.Items["__OriginalAuthTicket"] = authTicket;
				context.Items["__AuthenticationMethod"] = null;
			}

			return PipelineContinuation.Continue;
		}
	}
}