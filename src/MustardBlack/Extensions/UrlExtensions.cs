// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	
	static class UrlExtensions
	{
		// TODO: This shouldnt really be in MustardBlack, this is temporary whilst the usages are replaced with configurations
		public static string Domain(this Url url)
		{
			var firstDot = url.Host.IndexOf('.');
			if (firstDot == -1)
				return null;

			var domain = url.Host.Substring(firstDot);
			return domain;
		}
	}
}
