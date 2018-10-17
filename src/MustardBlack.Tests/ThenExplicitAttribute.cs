using System.Diagnostics;

namespace MustardBlack.Tests
{
	public class ThenExplicitAttribute : ThenAttribute
	{
		public ThenExplicitAttribute()
		{
			if (!Debugger.IsAttached)
				this.Skip = "Only running in interactive mode.";
		}
	}
}