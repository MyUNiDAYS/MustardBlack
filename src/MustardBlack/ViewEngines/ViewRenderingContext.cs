using System.IO;

namespace MustardBlack.ViewEngines
{
	public sealed class ViewRenderingContext
	{
		/// Gets or sets the <see cref="TextWriter"/> used to write the output.
		public TextWriter Writer { get; set; }
	}
}