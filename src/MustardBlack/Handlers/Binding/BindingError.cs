namespace MustardBlack.Handlers.Binding
{
	public sealed class BindingError
	{
		public readonly string ErrorKey;
		public readonly string ParameterKey;
		public readonly string AttemptedValue;

		public BindingError(string errorKey, string parameterKey, string propertyValue)
		{
			this.ErrorKey = errorKey;
			this.ParameterKey = parameterKey;
			this.AttemptedValue = propertyValue;
		}
	}
}