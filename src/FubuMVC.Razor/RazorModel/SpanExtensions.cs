using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser.SyntaxTree;
using FubuMVC.Razor.Core;

namespace FubuMVC.Razor.RazorModel
{
    public static class SpanExtensions
    {
        public static string Master(this IEnumerable<Span> chunks)
        {
            return chunks.Select(x => x.CodeGenerator).OfType<SetLayoutCodeGenerator>().FirstValue(x => x.LayoutPath);
        }

        public static string ViewModel(this IEnumerable<Span> chunks)
        {
            return chunks.Select(x => x.CodeGenerator).OfType<SetModelTypeCodeGenerator>().FirstValue(x => x.ModelType);
        }

        public static IEnumerable<string> Namespaces(this IEnumerable<Span> chunks)
        {
            return chunks.Select(x => x.CodeGenerator).OfType<AddImportCodeGenerator>().Select(x => x.Namespace.Trim());
        }
    }
}