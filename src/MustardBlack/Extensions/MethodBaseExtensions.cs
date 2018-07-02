using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;

// ReSharper disable CheckNamespace
namespace MustardBlack
{
	static class MethodBaseExtensions
	{
		/// <summary>
		/// Invokes a method on an instance with the given parameters, casting the result to the given type.
		/// This will never throw a TargetInvocationException
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="method"></param>
		/// <param name="instance"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static TResult InvokeWithoutThrowingTargetInvocationException<TResult>(this MethodBase method, object instance, params object[] arguments)
		{
			try
			{
				return (TResult)method.Invoke(instance, arguments);
			}
			catch (TargetInvocationException e)
			{
				var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e.InnerException);
				exceptionDispatchInfo.Throw();
				return default(TResult);
			}
		}

		/// <summary>
		/// Invokes a method on an instance with the given parameters, casting the result to the given type.
		/// This will never throw a TargetInvocationException
		/// </summary>
		/// <param name="method"></param>
		/// <param name="instance"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static void InvokeWithoutThrowingTargetInvocationException(this MethodBase method, object instance, params object[] arguments)
		{
			try
			{
				method.Invoke(instance, arguments);
			}
			catch (TargetInvocationException e)
			{
				var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e.InnerException);
				exceptionDispatchInfo.Throw();
			}
		}
	}
}