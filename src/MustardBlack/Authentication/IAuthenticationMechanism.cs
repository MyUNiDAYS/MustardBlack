using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;

namespace MustardBlack.Authentication
{
	public interface IAuthenticationMechanism
	{
		/// <summary>
		/// Attempt to identify a request. Should return null if the request cannot be identified in this method.
		/// </summary>
		Task<IAuthTicket> TryIdentify(PipelineContext context);

		/// <summary>
		/// Attempts to authenticate and authorize the identity
		/// </summary>
		/// <param name="context"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		Task<IResult> AuthenticateAndAuthorize(PipelineContext context, object attribute);

		/// <summary>
		/// Called after handler execution when this authentication method originally authenticated the request.
		/// </summary>
		Task SetResponse(PipelineContext context, IAuthTicket originalAuthTicket, IAuthTicket newAuthTicket);
	}
}
