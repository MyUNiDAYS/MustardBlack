using System;
using System.Collections.Generic;

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
		/// <param name="method"></param>
		/// <returns></returns>
		HandlerAction GetHandlerAction(Type handlerType, HttpMethod method);

		void Warm(IEnumerable<Type> handlerTypes);
	}
}
