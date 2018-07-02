using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute;

namespace MustardBlack.Tests
{
	public class AutoMocker
	{
		Dictionary<Type, object> mocks;
		Dictionary<Type, object> subjects;
		readonly object mutex;

		protected AutoMocker()
		{
			this.mutex = new object();
			this.Reset();
		}

		protected T Subject<T>() where T : class
		{
			lock (this.mutex)
			{

				if (!this.subjects.ContainsKey(typeof (T)))
					this.subjects.Add(typeof (T), this.CreateSubjectUnderTest<T>());
			}

			return this.subjects[typeof (T)] as T;
		}

		protected object Subject(Type t)
		{
			lock (this.mutex)
			{
				if (!this.subjects.ContainsKey(t))
					this.subjects.Add(t, this.CreateSubjectUnderTest(t));
			}

			return this.subjects[t];
		}

		protected void Reset()
		{
			lock (this.mutex)
			{
				this.mocks = new Dictionary<Type, object>();
				this.subjects = new Dictionary<Type, object>();
			}
		}

		protected virtual TMockedClass Dependency<TMockedClass>() where TMockedClass : class
		{
			return this.MockOfType(typeof (TMockedClass)) as TMockedClass;
		}

		protected static TMockedClass Stub<TMockedClass>() where TMockedClass : class
		{
			return Substitute.For<TMockedClass>();
		}

		protected virtual void Inject<TClass>(TClass instance) where TClass : class
		{
			this.mocks.Add(typeof (TClass), instance);
		}

		protected virtual T CreateSubjectUnderTest<T>() where T : class
		{
			return this.CreateSubjectUnderTest(typeof (T)) as T;
		}

		protected virtual object CreateSubjectUnderTest(Type t)
		{
			return Activator.CreateInstance(t, this.GetParameterObjects(t));
		}

		object[] GetParameterObjects(Type t)
		{
			var parameterObjects = new List<object>();
			var constructorInfo = GetGreediestConstructor(t);

			foreach (var parameter in constructorInfo.GetParameters())
				parameterObjects.Add(this.MockOfType(parameter.ParameterType));

			return parameterObjects.ToArray();
		}

		static ConstructorInfo GetGreediestConstructor(Type type)
		{
			return type.GetConstructors()
				.OrderByDescending(c => c.GetParameters().Length)
				.First();
		}

		object MockOfType(Type type)
		{
			this.EnsureStubExistsForType(type);
			return this.mocks[type];
		}

		void EnsureStubExistsForType(Type type)
		{
			if (!this.mocks.ContainsKey(type))
			{
				if (type.IsSealed)
					throw new ArgumentException("Can't create mocks of sealed classes: `" + type.FullName + "`");
				this.mocks.Add(type, Substitute.For(new [] { type }, new object[0]));
			}
		}
	}
}