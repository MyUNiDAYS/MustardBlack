using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace MustardBlack.ViewEngines.Razor
{
	/*
	sealed class DocumentClassifierPass : DocumentClassifierPassBase
	{
		protected override string DocumentKind => "MustardBlackView";

		protected override bool IsMatch(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode) => true;

		protected override void OnDocumentStructureCreated(RazorCodeDocument codeDocument, NamespaceDeclarationIntermediateNode @namespace, ClassDeclarationIntermediateNode @class, MethodDeclarationIntermediateNode method)
		{
			var compilationData = codeDocument.Items["ViewCompilationData"] as RazorViewCompilationData;

			@namespace.Content = compilationData.Namespace;
			@class.ClassName = compilationData.ClassName;

			@class.BaseType = "global::MustardBlack.ViewEngines.Razor.RazorViewPage";
			@class.Modifiers.Clear();
			@class.Modifiers.Add("public");

			method.MethodName = "ExecuteAsync";
			method.Modifiers.Clear();
			method.Modifiers.Add("public");
			method.Modifiers.Add("async");
			method.Modifiers.Add("override");
			method.ReturnType = $"global::{typeof(System.Threading.Tasks.Task).FullName}";
		}
	}
*/
}