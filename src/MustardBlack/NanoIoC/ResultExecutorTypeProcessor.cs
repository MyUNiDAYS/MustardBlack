using System;
using Microsoft.Extensions.DependencyInjection;
using MustardBlack.Results;
using NanoIoC;

namespace MustardBlack.NanoIoC
{
	/// <summary>
	/// Finds validators and registers them
	/// </summary>
	sealed class ResultExecutorTypeProcessor : OpenGenericTypeProcessor
	{
		protected override Type OpenGenericTypeToClose => typeof(IResultExecutor<>);

		public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
	}
}
