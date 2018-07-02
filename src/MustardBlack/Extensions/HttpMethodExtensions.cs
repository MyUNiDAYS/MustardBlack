// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	public static class HttpMethodExtensions
	{
		/// <summary>
		/// Determines if the method is GET, HEAD or OPTIONS
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static bool IsSafe(this HttpMethod method)
		{
			return method == HttpMethod.Get || method == HttpMethod.Head || method == HttpMethod.Options;
		}
	}
}
