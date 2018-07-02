using System;
using System.Collections.Generic;
using System.Reflection;

namespace MustardBlack.Handlers
{
	/// <summary>
	/// Routing helper class
	/// </summary>
	public sealed class HandlerAction
	{
		/// <summary>
		/// The method on the handler to invoke
		/// </summary>
		public readonly MethodInfo HandleMethod;

		/// <summary>
		/// Pipeline operators that require execution
		/// </summary>
		public readonly IEnumerable<PipelineOperator> Operators;

		/// <summary>
		/// Decorator Attributes on the Handler
		/// </summary>
		public readonly IEnumerable<IHandlerDecoratorAttribute> DecoratorAttributes;

		/// <summary>
		/// Creates a new HandlerAction
		/// </summary>
		/// <param name="handleMethod"></param>
		/// <param name="operators"></param>
		/// <param name="decoratorAttributes"></param>
		public HandlerAction(MethodInfo handleMethod, IEnumerable<PipelineOperator> operators, IEnumerable<IHandlerDecoratorAttribute> decoratorAttributes)
		{
			this.HandleMethod = handleMethod;
			this.Operators = operators;
			this.DecoratorAttributes = decoratorAttributes;
		}

		/// <summary>
		/// Helper class to describe a PipelineOperators exection
		/// </summary>
		public sealed class PipelineOperator
		{
			/// <summary>
			/// The type of the PipelineOperator to run for this attribute
			/// </summary>
			public readonly Type OperatorType;

			/// <summary>
			/// The order in which this should execute
			/// </summary>
			public readonly int Order;

			/// <summary>
			/// The attribute associated with the PipelineOperator registration
			/// </summary>
			public readonly Attribute Attribute;


			public PipelineOperator(int order, Attribute attribute, Type operatorType)
			{
				this.Order = order;
				this.Attribute = attribute;
				this.OperatorType = operatorType;
			}
		}
	}
}
