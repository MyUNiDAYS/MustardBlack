// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	static class StringExtensions
	{
		/// <summary>
		/// Trims a single leading character from a string, if present
		/// </summary>
		public static string TrimLeading(this string input, char character)
		{
			if (input.StartsWith(character.ToString()))
				return input.Substring(1);

			return input;
		}
	}
}