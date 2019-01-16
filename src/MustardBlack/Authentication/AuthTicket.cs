using System;

namespace MustardBlack.Authentication
{
	public sealed class AuthTicket : IAuthTicket
	{
		public string UserId { get; }
		public DateTime IssuedOn { get; }
		public bool IsIdentified => !string.IsNullOrEmpty(this.UserId);
		public string Token { get; }

		public AuthTicket()
		{
			this.IssuedOn = DateTime.UtcNow;
		}

		public AuthTicket(string userId, string token = null)
		{
			this.UserId = userId;
			this.Token = token ?? Guid.NewGuid().ToString();
			this.IssuedOn = DateTime.UtcNow;
		}

		public AuthTicket(string userId, DateTime issuedOn, string token = null)
		{
			this.IssuedOn = issuedOn;
			this.Token = token ?? Guid.NewGuid().ToString();
			this.UserId = userId;
		}

		bool Equals(AuthTicket other)
		{
			return this.UserId.Equals(other.UserId) && this.IssuedOn.Equals(other.IssuedOn) && string.Equals(this.Token, other.Token);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is AuthTicket && Equals((AuthTicket)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.UserId.GetHashCode();
				hashCode = (hashCode * 397) ^ this.IssuedOn.GetHashCode();
				hashCode = (hashCode * 397) ^ (this.Token != null ? this.Token.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}
