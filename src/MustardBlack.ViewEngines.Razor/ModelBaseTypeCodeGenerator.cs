using System.Globalization;
using System.Web.Razor.Generator;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class ModelBaseTypeCodeGenerator : SetBaseTypeCodeGenerator
	{
		public ModelBaseTypeCodeGenerator(string baseType) : base(baseType)
		{
		}

		protected override string ResolveType(CodeGeneratorContext context, string baseType)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}<{1}>", context.Host.DefaultBaseClass, baseType);
		} 
	}
}