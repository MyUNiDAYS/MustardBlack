using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;

namespace MustardBlack.Authentication
{
	/// <summary>
	/// Pipeline operator that authorizes the request.
	/// Unauthorized requests are redirected or return an appropriate JSON result based on whether the request was made via AJAX or not.
	/// </summary>
	public sealed class AuthorizePipelineOperator : AttributePipelineOperator<AuthorizeAttribute>
	{
		public override int Order => 5;

		public override async Task<PipelineContinuation> Operate(PipelineContext context)
		{
			var authenticationMethod = context.Items["__AuthenticationMethod"] as IAuthenticationMechanism;

			if (authenticationMethod != null)
			{
				context.Result = await authenticationMethod.AuthenticateAndAuthorize(context, this.Attribute);
				if (context.Result != null)
					return PipelineContinuation.SkipToPostHandler;

				return PipelineContinuation.Continue;
			}
			
			// Here we require auth, but really can't work out how to serve the user, so give default response
			context.Result = ErrorResult.Unauthorized();
			return PipelineContinuation.SkipToPostHandler;
		}
	}
}
