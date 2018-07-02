using System;

namespace MustardBlack
{
	public interface IBootstrapper : IDisposable
	{
		void Bootstrap();
	}
}
