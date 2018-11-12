using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MustardBlack.Build.Views
{
	static class AssemblyRepository
	{
		static IDictionary<string, Assembly> assemblies;

		public static void Initialize()
		{
			assemblies = new Dictionary<string, Assembly>();
			PreLoad();
		}
		
		public static void PreLoad()
		{
			try
			{
				var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (var assembly in loadedAssemblies)
				{
					if (!assembly.IsDynamic)
					{
						var key = Path.GetFileName(assembly.Location).ToLowerInvariant();
						// TODO: is this safe? Does this mean assemblies can be loaded twice?
						if (!assemblies.ContainsKey(key))
							assemblies.Add(key, assembly);
					}
				}

				AssembliesFromApplicationBaseDirectory();
			}
			catch (ReflectionTypeLoadException e)
			{
				throw new TypeLoadException(string.Join("\n\n", e.LoaderExceptions.Select(x => x.Message)), e);
			}
		}

		static void AssembliesFromApplicationBaseDirectory()
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			LoadAssembliesFromPath(baseDirectory);

//			string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
//			if (Directory.Exists(privateBinPath))
//				LoadAssembliesFromPath(privateBinPath);
		}

		public static void LoadAssembliesFromPath(string path)
		{
			var assemblyFiles = Directory.GetFiles(path).Where(file => Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase));

			foreach (var assemblyFile in assemblyFiles)
			{
				// Is this a sufficient unique id?
				var name = Path.GetFileName(assemblyFile).ToLowerInvariant();

				if (!assemblies.ContainsKey(name))
				{
					try
					{
						var assembly = Assembly.LoadFrom(assemblyFile);
						assemblies.Add(name, assembly);
					}
					catch (BadImageFormatException)
					{


					}
				}
			}
		}
	}
}