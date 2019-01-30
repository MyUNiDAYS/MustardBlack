using System;
using Microsoft.Extensions.DependencyInjection;
using MustardBlack.Pipeline;
using NanoIoC;

namespace MustardBlack.NanoIoC
{
	/// <summary>
	/// Finds AttributePipelineOperators and registers them
	/// </summary>
	sealed class AttributePipelineOperatorTypeProcessor : OpenGenericTypeProcessor
	{
		protected override Type OpenGenericTypeToClose => typeof(IAttributePipelineOperator<>);

		public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
	}
}
