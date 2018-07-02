using System;
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

		public override Lifecycle Lifecycle => Lifecycle.Singleton;
	}
}
