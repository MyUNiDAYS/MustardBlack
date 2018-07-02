using System;
using System.Collections.Generic;

namespace MustardBlack.Tests.Handlers.Binding
{
	public class MiddleClass
	{
		public InnerClass Inner { get; set; }
		public string Something { get; set; }

		public MiddleClass()
		{
			this.Inner = new InnerClass();
		}
	}

	public class OuterClass
	{
		public MiddleClass Middle { get; set; }
		public IList<MiddleClass> Middles { get; set; }

		public OuterClass()
		{
			this.Middle = new MiddleClass();
		}
	}

	public class InnerClass
	{
		public bool boolus { get; set; }
		public Guid guidus { get; set; }
		public string stringus { get; set; }
		public DateTime dateus { get; set; }
		public int intus { get; set; }
		public int? nullableInt { get; set; }
		public double doublus { get; set; }
		public IEnumerable<bool> enumerable { get; set; }
		public IEnumerable<string> strings { get; set; }
	}

    public class PrimitivesClass
    {
        public sbyte sbyteus { get; set; }
        public byte byteus { get; set; }
        public short shortus { get; set; }
        public ushort ushortus { get; set; }
        public int intus { get; set; }
        public uint uintus { get; set; }
        public long longus { get; set; }
        public ulong ulongus { get; set; }
        public char charus { get; set; }
        public float floatus { get; set; }
        public double doubleus { get; set; }
        public bool boolus { get; set; }
        public decimal decimalus { get; set; }
        public string stringus { get; set; }
    }
}