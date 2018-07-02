using System;

namespace MustardBlack.Authentication
{
	/// <summary>
	/// Denotes that the handler or method requires authorization
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class AuthorizeAttribute : Attribute
	{
	}
}
