using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MustardBlack.Pipeline;
using MustardBlack.Results;
using NanoIoC;
using Serilog;

namespace MustardBlack.Handlers
{
	public sealed class HandlerCache : IHandlerCache
	{
		readonly IResolverContainer container;
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
		readonly IDictionary<Type, IDictionary<HttpMethod, HandlerAction>> handlerMethodCache;

		public HandlerCache(IResolverContainer container)
		{
			this.container = container;
			this.handlerMethodCache = new Dictionary<Type, IDictionary<HttpMethod, HandlerAction>>();
		}

		public void Warm(IEnumerable<Type> handlerTypes)
		{
			foreach (var type in handlerTypes)
			{
				if (type.IsAbstract)
					continue;

				object[] customAttributes;
				try
				{
					customAttributes = type.GetCustomAttributes(true);
				}
				catch (Exception e)
				{
					log.Error(e, "Unable to get Attributes for Handler {type}, skipping Handler registration", type);
					continue;
				}

				var attributes = customAttributes.OfType<Attribute>().ToArray();
				var pipelineOperators = this.GetPipelineOperators(attributes);

				var hdAttributes = customAttributes.OfType<IHandlerDecoratorAttribute>().ToArray();

				var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

				// find all Http Method handling Methods
				var getMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "get", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));
				var postMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "post", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));
				var putMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "put", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));
				var patchMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "patch", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));
				var deleteMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "delete", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));
				var headMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "head", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));
				var optionsMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "options", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));
				var anyMethod = methodInfos.FirstOrDefault(m => string.Equals(m.Name, "any", StringComparison.InvariantCultureIgnoreCase) && (m.ReturnType.IsOrDerivesFrom(typeof(IResult)) || m.ReturnType.IsOrDerivesFrom(typeof(Task<IResult>))));

				var methodHash = new Dictionary<HttpMethod, HandlerAction>
				{
					{HttpMethod.Get, GetHandlerAction(getMethod, pipelineOperators, hdAttributes)},
					{HttpMethod.Post, GetHandlerAction(postMethod, pipelineOperators, hdAttributes)},
					{HttpMethod.Put, GetHandlerAction(putMethod, pipelineOperators, hdAttributes)},
					{HttpMethod.Patch, GetHandlerAction(patchMethod, pipelineOperators, hdAttributes)},
					{HttpMethod.Delete, GetHandlerAction(deleteMethod, pipelineOperators, hdAttributes)},
					{HttpMethod.Head, GetHandlerAction(headMethod, pipelineOperators, hdAttributes)},
					{HttpMethod.Options, GetHandlerAction(optionsMethod, pipelineOperators, hdAttributes)},
					{HttpMethod.All, GetHandlerAction(anyMethod, pipelineOperators, hdAttributes)}
				};
				this.handlerMethodCache.Add(type, methodHash);
			}
		}

		public IEnumerable<Type> AllHandlerTypes => this.handlerMethodCache.Keys.OrderBy(k => k.FullName).ToArray();

		/// <summary>
		/// Get the HttpMethods handled by the given handler type
		/// </summary>
		/// <param name="handlerType"></param>
		/// <returns></returns>
		public HttpMethod GetHandledMethods(Type handlerType)
		{
			if (!this.handlerMethodCache.ContainsKey(handlerType))
				return 0;

			var methods = this.handlerMethodCache[handlerType].Aggregate((HttpMethod)0, (method, pair) => pair.Value != null ? method | pair.Key : method);
			return methods;
		}

		/// <summary>
		/// Get the HandlerAction for the given handler method and global pipeline operators
		/// </summary>
		/// <param name="method"></param>
		/// <param name="operators"></param>
		/// <param name="hdAttributes"></param>
		/// <returns></returns>
		HandlerAction GetHandlerAction(MethodInfo method, IEnumerable<HandlerAction.PipelineOperator> operators, IEnumerable<IHandlerDecoratorAttribute> hdAttributes)
		{
			if (method == null)
				return null;

			var customAttributes = method.GetCustomAttributes(true).Cast<Attribute>();
			var pipelineOperators = this.GetPipelineOperators(customAttributes);

			pipelineOperators = pipelineOperators.Union(operators).OrderBy(x => x.Order).ToArray();

			return new HandlerAction(method, pipelineOperators, hdAttributes);
		}

		/// <summary>
		/// Get the handler/method specific PipelineOperators from the givne attributes
		/// </summary>
		/// <param name="handlerAttributes"></param>
		/// <returns></returns>
		IEnumerable<HandlerAction.PipelineOperator> GetPipelineOperators(IEnumerable<Attribute> handlerAttributes)
		{
			var pipelineOperators = new List<HandlerAction.PipelineOperator>();

			foreach (var attribute in handlerAttributes)
			{
				var attributeType = attribute.GetType();
				while (attributeType != typeof(Attribute))
				{
					var operatorType = typeof(IAttributePipelineOperator<>).MakeGenericType(attributeType);

					// Only register if the container knows about POs for the attribute
					if (this.container.HasRegistrationsFor(operatorType))
						pipelineOperators.Add(new HandlerAction.PipelineOperator(0, attribute, operatorType));

					attributeType = attributeType.BaseType;
				}
			}

			return pipelineOperators;
		}

		/// <summary>
		/// Finds the method on the ApiEndpoint to execute
		/// </summary>
		/// <param name="handlerType"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		public HandlerAction GetHandlerAction(Type handlerType, HttpMethod method)
		{
			var verbMethod = this.handlerMethodCache[handlerType][method];

			if (verbMethod != null)
				return verbMethod;

			if (method == HttpMethod.Head && this.handlerMethodCache[handlerType][HttpMethod.Get] != null)
				return this.handlerMethodCache[handlerType][HttpMethod.Get];

			verbMethod = this.handlerMethodCache[handlerType][HttpMethod.All];

			return verbMethod;
		}
	}
}
