// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace MustardBlack.ViewEngines.Razor.Internal
{
	sealed class CharArrayBufferSource : ICharBufferSource
	{
		public static readonly CharArrayBufferSource Instance = new CharArrayBufferSource();

		public char[] Rent(int bufferSize) => new char[bufferSize];

		public void Return(char[] buffer)
		{
			// Do nothing.
		}
	}
}
