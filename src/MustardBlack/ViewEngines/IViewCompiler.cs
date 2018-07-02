using System;

namespace MustardBlack.ViewEngines
{
	public interface IViewCompiler
	{
		Type Compile(string path);
	}
}