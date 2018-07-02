using System;

namespace MustardBlack.Authentication
{
	public interface IAuthTicket
	{
		/// <summary>
		/// Gets the Id of the currently logged in User, or Guid.Empty if the request isn't authenticated
		/// </summary>
		Guid UserId { get; }

		DateTime IssuedOn { get; }

		bool IsIdentified { get; }

		string Token { get; }
	}
}