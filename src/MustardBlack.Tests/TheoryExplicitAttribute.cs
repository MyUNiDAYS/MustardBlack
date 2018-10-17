using System.Diagnostics;
using Xunit;

namespace MustardBlack.Tests
{
	public class TheoryExplicitAttribute : TheoryAttribute
	{
		public TheoryExplicitAttribute()
		{
			if (!Debugger.IsAttached)
				this.Skip = "Only running in interactive mode.";
		}
	}
}