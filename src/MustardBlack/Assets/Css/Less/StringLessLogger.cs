using System.Text;
using dotless.Core.Loggers;

namespace MustardBlack.Assets.Css.Less
{
	public sealed class StringLessLogger : Logger
	{
		readonly StringBuilder stringBuilder;

		public StringLessLogger(LogLevel level)
			: base(level)
		{
			this.stringBuilder = new StringBuilder();
		}

		protected override void Log(string message)
		{
			this.stringBuilder.Append("/* ").Append(message).AppendLine(" */");
		}

		public string Dump()
		{
			return this.stringBuilder.ToString();
		}

		public bool HasMessages
		{
			get { return this.stringBuilder.Length > 0; }
		}

		public void Clear()
		{
			this.stringBuilder.Clear();
		}
	}
}