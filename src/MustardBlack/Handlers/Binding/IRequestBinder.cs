using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Hosting;
using MustardBlack.Routing;

namespace MustardBlack.Handlers.Binding
{
	/// <summary>
	/// Binds the Request to Handler Methods' parameters
	/// </summary>
	public interface IRequestBinder
	{
		BindingResult Bind(object owner, ParameterInfo parameterInfo, IRequest request, RouteValues routeValues);

		/// TODO: Violates CQS, fix this
		Task<object[]> GetAndValidateParameters(object owner, MethodInfo verbMethod, IRequest request, RouteValues routeValues);
	}
}