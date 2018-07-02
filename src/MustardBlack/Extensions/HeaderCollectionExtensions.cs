using MustardBlack.Hosting;

// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	/// <summary>
	/// Extension methods for the IRequest interface
	/// </summary>
	public static class HeaderCollectionExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static string Origin(this HeaderCollection headers)
		{
			if (headers["Origin"] == "null" || headers["Origin"] == "file://")
				return null;

			return headers["Origin"];
		}
    }
}