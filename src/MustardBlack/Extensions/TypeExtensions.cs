using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MustardBlack
{
	static class TypeExtensions
	{
		/// <summary>
		/// Determines if the current type is or derives from the given type.
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static bool IsOrDerivesFrom<TOther>(this Type self)
		{
			return self.IsOrDerivesFrom(typeof (TOther));
		}

		/// <summary>
		/// Determines if the current type is or derives from the given type.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool IsOrDerivesFrom(this Type self, Type other)
		{
			// same
			if (self.Equals(other))
				return true;

			// derived classes
			if (self.IsSubclassOf(other))
				return true;

			if (other.IsAssignableFrom(self))
				return true;

			// interfaces
			IEnumerable<Type> interfaces = self.GetInterfaces();
			if (self.IsInterface)
				interfaces = interfaces.Union(new[] { self });

			foreach (var face in interfaces)
			{
				if (face.IsGenericType)
				{
					var genericFace = face.GetGenericTypeDefinition();
					if (genericFace.IsAssignableFrom(other))
						return true;
				}
			}

			// generic base classes
			var baseType = self;
			while (baseType != null && baseType != typeof(object))
			{
				if (baseType.IsGenericType)
				{
					var genericBase = baseType.GetGenericTypeDefinition();
					if (genericBase.IsAssignableFrom(other))
						return true;
				}

				baseType = baseType.BaseType;
			}

			return false;
		}

		/// <summary>
		/// Gets the arguments closing the given open generic type, from the current types hierarchy
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static Type[] GetGenericArgumentsClosing(this Type self, Type other)
		{
			if (other.IsInterface)
			{
				if (!other.IsGenericType)
					throw new ArgumentException("Must be open generic", nameof(other));

				var genericDefinition = other.GetGenericTypeDefinition();
				if (genericDefinition != other)
					throw new ArgumentException("Must be open generic", nameof(other));

				var interfaces = self.GetInterfaces();
				foreach (var @interface in interfaces)
				{
					if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == other)
						return @interface.GetGenericArguments();
				}

				throw new ArgumentException("Current type does not close other type", nameof(other));
			}

			return null;
		}

		public static object Activate(this Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch (MissingMethodException e)
			{
				throw new Exception("Cannot create `" + type + "`", e);
			}
		}

		public static bool CanBeActivated(this Type self)
		{
			if (self.IsAbstract)
				return false;

			return self.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Any(c => c.GetParameters().All(p => p.IsOptional));

		}
		
		public static bool MethodReturnsTask(this MethodInfo method)
		{
			return method.ReturnType.IsOrDerivesFrom(typeof (Task));
		}

		public static bool IsStatic(this Type type)
		{
			return type.IsAbstract && type.IsSealed;
		}
	}
}