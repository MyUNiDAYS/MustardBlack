namespace MustardBlack.Hosting
{
	public sealed class ContentType : System.Net.Mime.ContentType
	{
		public ContentType()
		{
		}

		public ContentType(string contentType) : base(contentType)
		{
		}

		public static implicit operator ContentType(string contentType)
		{
			return new ContentType(contentType);
		}
	}
}