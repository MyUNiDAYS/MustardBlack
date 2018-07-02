namespace MustardBlack
{
	/// <summary>
	/// Different types of request
	/// </summary>
	public enum RequestType
	{
		/// <summary>
		/// Request was made directly. Normal browser/HTTP request
		/// </summary>
		Direct = 1,

		/// <summary>
		/// Request was made via AJAX
		/// </summary>
		Ajax = 2
	}
}