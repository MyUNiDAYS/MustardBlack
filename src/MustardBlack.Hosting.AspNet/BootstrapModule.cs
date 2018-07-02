using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;

namespace MustardBlack.AspNet
{
	public sealed class BootstrapModule : IHttpModule
	{
		static volatile bool booted;
		static readonly object mutex = new object();
		IBootstrapper bootstrapper;

		public void Init(HttpApplication context)
		{
			if (booted)
				return;

			lock (mutex)
			{
				if (booted)
					return;

				try
				{
					var bootstrapperType = BuildManager.GetReferencedAssemblies()
						.Cast<Assembly>()
						.SelectMany(a => a.GetTypes())
						.Where(t => !t.IsAbstract)
						.FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IBootstrapper)));

					if (bootstrapperType != null)
					{
						this.bootstrapper = Activator.CreateInstance(bootstrapperType) as IBootstrapper;
						this.bootstrapper?.Bootstrap();
					}
				}
				catch (ReflectionTypeLoadException e)
				{
					throw new BootException(e.LoaderExceptions, e);
				}
				finally
				{
					booted = true;
				}
			}
		}
		
		public void Dispose()
		{
			this.bootstrapper?.Dispose();
		}
	}
}
