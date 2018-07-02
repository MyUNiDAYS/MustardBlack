using System.Collections.Generic;

namespace MustardBlack.Hosting
{
	public sealed class RequestState : IRequestState
	{
		public IDictionary<string, IList<string>> Errors { get; }
		public IDictionary<string, IList<string>> ValidationErrors { get; }
		public IDictionary<string, string> AttemptedValues { get; }
		public bool IsValid => this.Errors.Count == 0;

		public RequestState()
		{
			this.Errors = new Dictionary<string, IList<string>>();
			this.ValidationErrors = new Dictionary<string, IList<string>>();
			this.AttemptedValues = new Dictionary<string, string>();
		}

		public void AddError(string key, string error)
		{
			if(!this.Errors.ContainsKey(key))
				this.Errors.Add(key, new List<string>());

			this.Errors[key].Add(error);
		}

		public void AddValidationError(string key, string errorKey, string error)
		{
			if (!this.ValidationErrors.ContainsKey(key))
				this.ValidationErrors.Add(key, new List<string>());

			this.AddError(key, error);

			this.ValidationErrors[key].Add(errorKey);
		}

		public void AddError(string error)
		{
			this.AddError(string.Empty, error);
		}

		public bool HasErrorFor(string key)
		{
			return this.Errors.ContainsKey(key);
		}
	}
}
