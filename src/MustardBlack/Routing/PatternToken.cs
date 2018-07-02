namespace MustardBlack.Routing
{
	sealed class PatternToken
	{
		public PatternTokenType Type { get; }
		public string Name { get; }

		public PatternToken(PatternTokenType type, string name)
		{
			this.Type = type;
			this.Name = name;
		}
	}
}