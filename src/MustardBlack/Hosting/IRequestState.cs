using System.Collections.Generic;

namespace MustardBlack.Hosting
{
	public interface IRequestState
	{
		IDictionary<string, IList<string>> Errors { get; }
		IDictionary<string, IList<string>> ValidationErrors { get; }
		IDictionary<string, string> AttemptedValues { get; }
		bool IsValid { get; }
		void AddError(string key, string error);
		void AddError(string error);
		bool HasErrorFor(string key);
		void AddValidationError(string key, string errorKey, string error);
	}
}
