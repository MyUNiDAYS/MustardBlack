using System.Globalization;
using MuonLab.Validation;

namespace MustardBlack.Example
{
	sealed class DefaultErrorMessageResolver : IErrorMessageResolver
	{
		public string GetErrorMessage(ErrorDescriptor error, CultureInfo culture)
		{
			return error.Key;
		}
	}
}