using System.Globalization;

namespace MustardBlack
{
	public sealed class HrefLang
	{
		public readonly string RegionCode;
		public readonly CultureInfo CultureCode;

		public HrefLang(string regionCode, CultureInfo cultureCode)
		{
			this.RegionCode = regionCode;
			this.CultureCode = cultureCode;
		}
	}
}