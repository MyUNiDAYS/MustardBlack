using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;

namespace MustardBlack.ViewEngines.Razor
{
	public sealed class DefaultDirectivesProjectItem : RazorProjectItem
	{
		readonly byte[] defaultImportBytes;

		public DefaultDirectivesProjectItem(IEnumerable<string> defaultNamespaces, IEnumerable<Type> defaultTagHelpers)
		{
			var preamble = Encoding.UTF8.GetPreamble();
			var content = @"
@using System
@using System.Collections.Generic
@using System.Linq
@using System.Threading.Tasks
@using MustardBlack.ViewEngines
@using Microsoft.CSharp.RuntimeBinder
" + string.Join("\n", defaultNamespaces.Select(n => "@using " + n + ";")) + @"
" + string.Join("\n", defaultTagHelpers.Select(t => "@addTagHelper " + t.Name + ", " + t.Assembly.GetName().Name));
			var contentBytes = Encoding.UTF8.GetBytes(content);

			this.defaultImportBytes = new byte[preamble.Length + contentBytes.Length];
			preamble.CopyTo(this.defaultImportBytes, 0);
			contentBytes.CopyTo(this.defaultImportBytes, preamble.Length);
		}

		public override string BasePath => null;

		public override string FilePath => null;

		public override string PhysicalPath => null;

		public override bool Exists => true;

		public override Stream Read() => new MemoryStream(this.defaultImportBytes);
	}
}