namespace MustardBlack.Hosting.AspNetCore
{
	public class BootException : Exception
	{
		public BootException(IEnumerable<Exception> exceptions, Exception innerException) : base("Unable to boot applicaiton\n" + string.Join("\n", exceptions.Select(e => e.Message)), innerException)
		{
		}

		protected BootException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
