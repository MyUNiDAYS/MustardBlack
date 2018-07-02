using System;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class AttributeValue
	{
		public Tuple<string, int> Prefix { get; }
		public Tuple<object, int> Value { get; }
		public bool IsLiteral { get; }

		public AttributeValue(Tuple<string, int> prefix, Tuple<object, int> value, bool isLiteral)
		{
			this.Prefix = prefix;
			this.Value = value;
			this.IsLiteral = isLiteral;
		}

		public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<object, int>, bool> value)
		{
			return new AttributeValue(value.Item1, value.Item2, value.Item3);
		}

		public static implicit operator AttributeValue(Tuple<Tuple<string, int>, Tuple<string, int>, bool> value)
		{
			return new AttributeValue(value.Item1, new Tuple<object, int>(value.Item2.Item1, value.Item2.Item2), value.Item3);
		}
	}
}