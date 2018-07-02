namespace MustardBlack
{
	public sealed class RequestCookie
	{
		public readonly string Name;
		public readonly string Value;

		public RequestCookie(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}
	}
}
