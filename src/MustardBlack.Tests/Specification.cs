using System;

namespace MustardBlack.Tests
{
	public abstract class Specification : IDisposable
	{
		Type expectedExceptionType;
		Exception thrownException;

		protected Specification()
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

		public virtual void Dispose()
		{
		}
	}
}