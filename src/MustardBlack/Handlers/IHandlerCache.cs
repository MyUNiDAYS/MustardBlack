using System;
using System.Collections.Generic;
using MustardBlack.Hosting;

namespace MustardBlack.Handlers
{
	public interface IHandlerCache
	{
		IEnumerable<Type> AllHandlerTypes { get; }

		HttpMethod GetHandledMethods(Type handlerType);

		/// <summary>
		/// Finds the method on the ApiEndpoint to execute
		/// </summary>
		/// <param name="handlerType"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		HandlerAction GetHandlerAction(Type handlerType, IRequest request);

		void Warm(IEnumerable<Type> handlerTypes);
	}
}
