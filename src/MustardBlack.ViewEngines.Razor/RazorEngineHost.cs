using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class RazorEngineHost : System.Web.Razor.RazorEngineHost
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RazorEngineHost"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
		public RazorEngineHost(RazorCodeLanguage language) : base(language)
        {
            this.DefaultBaseClass = typeof (RazorViewPage).FullName;
            this.DefaultNamespace = "RazorOutput";
            this.DefaultClassName = "RazorView";

            var context = new GeneratedClassContext("Execute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo", null, "DefineSection");
            context.ResolveUrlMethodName = "ResolveUrl";

            this.GeneratedClassContext = context;
        }
		
		/// <summary>
		/// Decorates the code parser.
		/// </summary>
		/// <param name="incomingCodeParser">The incoming code parser.</param>
		/// <returns></returns>
		public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
		{
			if (incomingCodeParser is CSharpCodeParser)
				return new RazorCodeParser();

			return base.DecorateCodeParser(incomingCodeParser);
		}

	}
}