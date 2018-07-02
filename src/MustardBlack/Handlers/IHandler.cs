using System.Collections.Generic;
using MustardBlack.Hosting;
using MustardBlack.Pipeline;

namespace MustardBlack.Handlers
{
	/// <summary>
	/// Marker indicating that implementors are Handlers
	/// </summary>
	public interface IHandler
	{
		PipelineContext Context { set; }
		IEnumerable<HrefLang> GetAlternateLangs(IRequest request);
	}
}