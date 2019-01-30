using System;
using Microsoft.Extensions.DependencyInjection;
using NanoIoC;

namespace MustardBlack.NanoIoC
{
	sealed class DefaultTypeProcessor : ITypeProcessor
	{
		public void Process(Type type, IContainer container)
		{
			if (!type.IsClass || type.IsAbstract || !type.Assembly.FullName.StartsWith("MustardBlack"))
				return;

			var interfaces = type.GetInterfaces();

			foreach (var face in interfaces)
			{
				if (face.Name == "I" + type.Name)
					container.Register(face, type, ServiceLifetime.Singleton);
			}
		}
	}
}