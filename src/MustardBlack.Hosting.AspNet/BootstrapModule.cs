using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using MustardBlack.Extensions;

namespace MustardBlack.Hosting.AspNet
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
				catch (TypeInitializationException e)
				{
					throw e.CreateDetailedException();
				}
				catch (ReflectionTypeLoadException e)
				{
					throw e.CreateDetailedException();
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
