using System;

namespace MustardBlack
{
	public sealed class ResponseCookie
	{
		public string Name { get; }
		public string Value { get; }
		public DateTime? Expires { get; }
		public bool Secure { get; }
		public bool HttpOnly { get; }
		public string Domain { get; }
		public string Path { get; }

		public ResponseCookie(string name, string value = null, DateTime? expires = null, bool secure = true, bool httpOnly = true, string domain = null, string path = "/")
		{
			this.Name = name;
			this.Value = value;
			this.Expires = expires;
			this.Secure = secure;
			this.HttpOnly = httpOnly;
			this.Domain = domain;
			this.Path = path;
		}
	}
}
