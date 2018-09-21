using System.IO;

namespace MustardBlack.ViewEngines.Razor.Internal
{
	public class RazorViewContext
	{
		/// <summary>
		/// Gets or sets the <see cref="TextWriter"/> used to write the output.
		/// </summary>
		public TextWriter Writer { get; set; }
	}
}