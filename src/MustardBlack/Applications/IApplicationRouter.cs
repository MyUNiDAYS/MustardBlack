using System.Threading.Tasks;
using MustardBlack.Pipeline;

namespace MustardBlack.Applications
{
	/// <summary>
	/// Routes a request to the first Application that can serve it, and has the Application Serve the request
	/// </summary>
	public interface IApplicationRouter
	{
		Task RouteApplication(PipelineContext request);
	}
}
