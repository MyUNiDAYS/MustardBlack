using System.Threading.Tasks;
using MustardBlack.Pipeline;

namespace MustardBlack.Authentication
{
	public sealed class AuthenticationPostHandlerExecutionPipelineOperator : IPostHandlerExecutionPipelineOperator
	{
		public async Task<PipelineContinuation> Operate(PipelineContext context)
		{
			if(!context.Items.ContainsKey("__AuthenticationMethod"))
				return PipelineContinuation.Continue;

			var authMethod = context.Items["__AuthenticationMethod"] as IAuthenticationMechanism;
			if(authMethod == null)
				return PipelineContinuation.Continue;

			var originalAuthTicket = context.Items["__OriginalAuthTicket"] as IAuthTicket;
			var newAuthTicket = context.GetAuthTicket();

			await authMethod.SetResponse(context, originalAuthTicket, newAuthTicket);

			return PipelineContinuation.Continue;
		}
	}
}
