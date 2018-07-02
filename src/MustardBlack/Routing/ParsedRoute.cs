using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Serilog;

namespace MustardBlack.Routing
{
	sealed class ParsedRoute
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		struct PatternSegment
		{
			public bool AllTokensAreLiteral;
			public List<PatternToken> Tokens;
		}

		static readonly char[] placeholderDelimiters = {'{', '}'};

		PatternSegment[] segments;
		Dictionary<string, bool> parameterNames;
		PatternToken[] tokens;

		int segmentCount;
		bool hasCatchAllSegment;
		
		public string Url { get; private set; }
		public bool IsDynamic { get; private set; }

		ParsedRoute()
		{
		}

		public static ParsedRoute Parse(string url)
		{
			var parsedRoute = new ParsedRoute();
			parsedRoute.Url = url;
			parsedRoute.parameterNames = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

			if (!string.IsNullOrEmpty(url))
			{
				if (url.IndexOf('?') >= 0)
					throw new ArgumentException("Url must not contain '?'");
			}
			else
			{
				parsedRoute.segments = new PatternSegment[0];
				parsedRoute.tokens = new PatternToken[0];
				return parsedRoute;
			}

			var parts = url.TrimStart('/').Split('/');
			var partsCount = parsedRoute.segmentCount = parts.Length;
			var allTokens = new List<PatternToken>();

			parsedRoute.segments = new PatternSegment[partsCount];

			for (var i = 0; i < partsCount; i++)
			{
				if (parsedRoute.hasCatchAllSegment)
					throw new ArgumentException("A catch-all parameter can only appear as the last segment of the route URL");

				var catchAlls = 0;
				var part = parts[i];
				var partLength = part.Length;
				var tokens = new List<PatternToken>();

				if (partLength == 0 && i > 0 && i < partsCount - 1)
					throw new ArgumentException("Consecutive URL segment separators '/' are not allowed");

				if (part.IndexOf("{}") != -1)
					throw new ArgumentException("Empty URL parameter name is not allowed");

				if (i > 0)
					allTokens.Add(null);

				PatternToken tmpToken;
				if (part.IndexOfAny(placeholderDelimiters) == -1)
				{
					// no placeholders here, short-circuit it
					tmpToken = new PatternToken(PatternTokenType.Literal, part);
					tokens.Add(tmpToken);
					allTokens.Add(tmpToken);
					parsedRoute.segments[i].AllTokensAreLiteral = true;
					parsedRoute.segments[i].Tokens = tokens;
					continue;
				}

				parsedRoute.IsDynamic = true;

				var currentIndex = 0;
				var allLiteral = true;
				while (currentIndex < partLength)
				{
					var openParameterIndex = part.IndexOf('{', currentIndex);
					if (openParameterIndex >= partLength - 2)
						throw new ArgumentException("Unterminated URL parameter. It must contain matching '}'");

					// No open tag, must be a literal
					if (openParameterIndex < 0)
					{
						if (part.IndexOf('}', currentIndex) >= currentIndex)
							throw new ArgumentException("Unmatched URL parameter closer '}'. A corresponding '{' must precede");
						var tmp = part.Substring(currentIndex);
						tmpToken = new PatternToken(PatternTokenType.Literal, tmp);
						tokens.Add(tmpToken);
						allTokens.Add(tmpToken);
						break;
					}

					// parameter found later in the segment, this bit is a literal
					if (currentIndex == 0 && openParameterIndex > 0)
					{
						tmpToken = new PatternToken(PatternTokenType.Literal, part.Substring(0, openParameterIndex));
						tokens.Add(tmpToken);
						allTokens.Add(tmpToken);
					}

					var end = part.IndexOf('}', openParameterIndex + 1);
					var next = part.IndexOf('{', openParameterIndex + 1);

					if (end < 0 || next >= 0 && next < end)
						throw new ArgumentException($"Unterminated URL parameter `{url}`. It must contain matching '}}'");

					if (next == end + 1)
						throw new ArgumentException($"Two consecutive URL parameters are not allowed `{url}`. Split into a different segment by '/', or a literal string.");

					if (next == -1)
						next = partLength;

					var token = part.Substring(openParameterIndex + 1, end - openParameterIndex - 1);
					PatternTokenType type;
					if (token[0] == '*')
					{
						catchAlls++;
						parsedRoute.hasCatchAllSegment = true;
						type = PatternTokenType.CatchAll;
						token = token.Substring(1);
					}
					else
						type = PatternTokenType.Standard;

					if (!parsedRoute.parameterNames.ContainsKey(token))
						parsedRoute.parameterNames.Add(token, true);

					tmpToken = new PatternToken(type, token);
					tokens.Add(tmpToken);
					allTokens.Add(tmpToken);
					allLiteral = false;

					if (end < partLength - 1)
					{
						token = part.Substring(end + 1, next - end - 1);
						tmpToken = new PatternToken(PatternTokenType.Literal, token);
						tokens.Add(tmpToken);
						allTokens.Add(tmpToken);
						end += token.Length;
					}

					if (catchAlls > 1 || (catchAlls == 1 && tokens.Count > 1))
						throw new ArgumentException("A path segment that contains more than one section, such as a literal section or a parameter, cannot contain a catch-all parameter.");
					currentIndex = end + 1;
				}

				parsedRoute.segments[i].AllTokensAreLiteral = allLiteral;
				parsedRoute.segments[i].Tokens = tokens;
			}

			if (allTokens.Count > 0)
				parsedRoute.tokens = allTokens.ToArray();

			return parsedRoute;
		}

		public RouteValues Match(string path)
		{
			var ret = new RouteValues();
			string url = this.Url;
			string[] argSegs;
			int argsCount;

			if (String.IsNullOrEmpty(path))
			{
				argSegs = null;
				argsCount = 0;
			}
			else
			{
				// TODO: cache indexof
				if (string.Compare(url, path, StringComparison.Ordinal) == 0 && url.IndexOf('{') < 0)
					return new RouteValues();

				argSegs = path.TrimLeading('/').Split('/');
				argsCount = argSegs.Length;
			}

			if (argsCount == 1 && String.IsNullOrEmpty(argSegs[0]))
				argsCount = 0;

			if ((this.hasCatchAllSegment && argsCount < this.segmentCount) || (!this.hasCatchAllSegment && argsCount != this.segmentCount))
				return null;

			int i = 0;

			foreach (PatternSegment segment in this.segments)
			{
				if (i >= argsCount)
					break;

				if (segment.AllTokensAreLiteral)
				{
					if (String.Compare(argSegs[i], segment.Tokens[0].Name, StringComparison.OrdinalIgnoreCase) != 0)
						return null;
					i++;
					continue;
				}

				string pathSegment = argSegs[i];
				int pathSegmentLength = pathSegment != null ? pathSegment.Length : -1;
				int pathIndex = 0;
				PatternTokenType tokenType;
				List<PatternToken> tokens = segment.Tokens;
				int tokensCount = tokens.Count;

				// Process the path segments ignoring the defaults
				for (int tokenIndex = 0; tokenIndex < tokensCount; tokenIndex++)
				{
					var token = tokens[tokenIndex];
					if (pathIndex > pathSegmentLength - 1)
						return null;

					tokenType = token.Type;
					var tokenName = token.Name;

					// Catch-all
					if (i > this.segmentCount - 1 || tokenType == PatternTokenType.CatchAll)
					{
						if (tokenType != PatternTokenType.CatchAll)
							return null;

						StringBuilder sb = new StringBuilder();
						for (int j = i; j < argsCount; j++)
						{
							if (j > i)
								sb.Append('/');
							sb.Append(argSegs[j]);
						}

						ret.Add(tokenName, sb.ToString());
						break;
					}

					// Literal sections
					if (token.Type == PatternTokenType.Literal)
					{
						int nameLen = tokenName.Length;
						if (pathSegmentLength < nameLen || String.Compare(pathSegment, pathIndex, tokenName, 0, nameLen, StringComparison.OrdinalIgnoreCase) != 0)
							return null;
						pathIndex += nameLen;
						continue;
					}

					int nextTokenIndex = tokenIndex + 1;
					if (nextTokenIndex >= tokensCount)
					{
						// Last token
						ret.Add(tokenName, pathSegment.Substring(pathIndex));
						continue;
					}

					// Next token is a literal - greedy matching. It seems .NET
					// uses a simple and naive algorithm here which finds the
					// last ocurrence of the next section literal and assigns
					// everything before that to this token. See the
					// GetRouteData28 test in RouteTest.cs
					var nextToken = tokens[nextTokenIndex];
					string nextTokenName = nextToken.Name;
					int lastIndex = pathSegment.LastIndexOf(nextTokenName, pathSegmentLength - 1, pathSegmentLength - pathIndex, StringComparison.OrdinalIgnoreCase);
					if (lastIndex == -1)
						return null;

					int copyLength = lastIndex - pathIndex;
					string sectionValue = pathSegment.Substring(pathIndex, copyLength);
					if (String.IsNullOrEmpty(sectionValue))
						return null;

					ret.Add(tokenName, sectionValue);
					pathIndex += copyLength;
				}
				i++;
			}

			if (i < this.segmentCount)
				return null;

			return ret;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userValues"></param>
		/// <returns></returns>
		public string BuildPath(RouteValues userValues)
		{
			userValues = userValues ?? new RouteValues();

			if (this.parameterNames.Any(de => !userValues.ContainsKey(de.Key)))
			{
				log.Error("User route values did not contain a necessary parameter to build path. Url: {url}, User values: {userValues}", this.Url, userValues.Keys);
				return null;
			}

			var urlBuilder = new StringBuilder();
			
			int tokensCount = this.tokens.Length - 1;
			for (int i = tokensCount; i >= 0; i--)
			{
				PatternToken token = this.tokens[i];
				if (token == null)
				{
					if (i < tokensCount && urlBuilder.Length > 0 && urlBuilder[0] != '/')
						urlBuilder.Insert(0, '/');
					continue;
				}

				if (token.Type == PatternTokenType.Literal)
				{
					urlBuilder.Insert(0, token.Name);
					continue;
				}

				var tokenValue = userValues[token.Name];
				
				if (tokenValue != null)
					urlBuilder.Insert(0, tokenValue.ToString());
			}

			urlBuilder.Insert(0, '/');
			return urlBuilder.ToString();
		}
	}
}