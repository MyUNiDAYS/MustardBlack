namespace MustardBlack.Assets
{
	public sealed class AssetProcessingResult
	{
		public CompilationStatus Status { get; }
		public string Result { get; }
		public string Message { get; }

		public AssetProcessingResult(CompilationStatus status, string result = null, string message = null)
		{
			this.Status = status;
			this.Result = result;
			this.Message = message;
		}

		public enum CompilationStatus
		{
			Success = 1,
			Failure = 2,
			Skipped = 3
		}
	}
}
