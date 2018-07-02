using System.Web.Razor.Parser;
using System.Web.Razor.Text;

namespace MustardBlack.ViewEngines.Razor
{
	public class RazorCodeParser : CSharpCodeParser
	{
		bool modelStatementFound;
		SourceLocation? endInheritsLocation;

		/// <summary>
		/// Initializes a new instance of the <see cref="RazorCodeParser"/> class.
		/// </summary>
		public RazorCodeParser()
		{
			this.MapDirectives(this.ModelDirective, "model");
		}

		protected virtual void ModelDirective()
		{
			this.AssertDirective("model");

			this.AcceptAndMoveNext();

			var endModelLocation = this.CurrentLocation;

			this.BaseTypeDirective("The 'model' keyword must be followed by a type name on the same line.", s => new ModelBaseTypeCodeGenerator(s));

			if (this.modelStatementFound)
				this.Context.OnError(endModelLocation, "Cannot have more than one @model statement.");

			this.modelStatementFound = true;

			this.CheckForInheritsAndModelStatements();
		}

		protected override void InheritsDirective()
		{
			this.AssertDirective("inherits");
			this.AcceptAndMoveNext();

			this.endInheritsLocation = this.CurrentLocation;

			base.InheritsDirective();

			this.CheckForInheritsAndModelStatements();
		}

		private void CheckForInheritsAndModelStatements()
		{
			if (this.modelStatementFound && this.endInheritsLocation.HasValue)
				this.Context.OnError(this.endInheritsLocation.Value, "Cannot have both an @inherits statement and an @model statement.");
		}
	}
}