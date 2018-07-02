using System;
using NUnit.Framework;

namespace MustardBlack.Tests
{
	[TestFixture]
	public abstract class AutoMockedSpecification : AutoMocker
	{
		Type expectedExceptionType;
		Exception thrownException;

		[TestFixtureSetUp]
		public void SetUp()
		{
			this.Given();

			try
			{
				this.When();
			}
			catch (Exception e)
			{
				if (this.expectedExceptionType == null || !e.GetType().IsOrDerivesFrom(this.expectedExceptionType))
					throw;

				this.thrownException = e;
			}
		}

		protected void Expect<TException>() where TException : Exception
		{
			this.expectedExceptionType = typeof(TException);
		}

		protected TException Thrown<TException>() where TException : Exception
		{
			return this.thrownException as TException;
		}

		protected virtual void Given() { }
		protected abstract void When();

		[TestFixtureTearDown]
		public virtual void TidyUp()
		{
		}
	}
}