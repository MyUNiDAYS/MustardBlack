using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace MustardBlack.Hosting
{
	// This exists so that we can write convenient extesions methods on the header collection
	public sealed class HeaderCollection : NameValueCollection
	{
		public HeaderCollection()
		{
		}

		public HeaderCollection(NameValueCollection col) : base(col)
		{
		}
		
		public HeaderCollection(int capacity) : base(capacity)
		{
		}

		public HeaderCollection(IEqualityComparer equalityComparer) : base(equalityComparer)
		{
		}

		public HeaderCollection(int capacity, IEqualityComparer equalityComparer) : base(capacity, equalityComparer)
		{
		}

		public HeaderCollection(int capacity, NameValueCollection col) : base(capacity, col)
		{
		}
		
		public HeaderCollection(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
