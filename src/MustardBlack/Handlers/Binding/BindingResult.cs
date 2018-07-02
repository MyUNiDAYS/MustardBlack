using System.Collections.Generic;

namespace MustardBlack.Handlers.Binding
{
	public sealed class BindingResult
	{
		public readonly object Object;

		public ResultType Result { get; }

		public readonly IEnumerable<BindingError> BindingErrors;

		public BindingResult(object @object, ResultType result = ResultType.Success)
		{
			this.Object = @object;
			this.BindingErrors = new BindingError[0];
			this.Result = result;
		}

		public BindingResult(object @object, params BindingError[] bindingErrors)
		{
			this.Object = @object;
			this.BindingErrors = bindingErrors ?? new BindingError[0];
			this.Result = ResultType.Failure;
		}

		public BindingResult(object @object, IEnumerable<BindingError> bindingErrors, ResultType result = ResultType.Failure)
		{
			this.Object = @object;
			this.BindingErrors = bindingErrors ?? new BindingError[0];
			this.Result = result;
		}

		public enum ResultType
		{
			Failure,
			Default,
			Success
		}
	}
}