// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace MustardBlack.ViewEngines.Razor.Internal
{
	public interface ICharBufferSource
	{
		char[] Rent(int bufferSize);

		void Return(char[] buffer);
	}
}
