using System;

namespace MustardBlack
{
	[Flags]
	public enum HttpMethod
	{
		Get = 1,
		Post = 2,
		Put = 4,
		Delete = 8,
		Head = 16,
		Options = 32,
		Patch = 64,

		All = Get | Post | Put | Delete | Head | Options | Patch
	}
}