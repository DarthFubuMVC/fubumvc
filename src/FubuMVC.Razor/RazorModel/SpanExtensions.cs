using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Parser.SyntaxTree;
using RazorEngine.Spans;

namespace FubuMVC.Razor.RazorModel
{
    public static class SpanExtensions
    {
        public static string Master(this IEnumerable<Span> chunks)
        {
            var codeBlock = chunks.OfType<CodeSpan>().FirstOrDefault(x => x.Content.Contains("_Layout"));
            if (codeBlock == null)
                return null;
            var codeBlockContent = codeBlock.Content;
            var layoutIndex = codeBlockContent.IndexOf("_Layout", StringComparison.Ordinal);
            var endLayoutIndex = codeBlockContent.IndexOf(';', layoutIndex);
            var layoutSlice = codeBlockContent.Substring(layoutIndex, endLayoutIndex - layoutIndex);
            var layoutValueStart = layoutSlice.IndexOf('"') + 1;
            var layoutValueEnd = layoutSlice.IndexOf('"', layoutValueStart);
            var layoutName = layoutSlice.Substring(layoutValueStart, layoutValueEnd - layoutValueStart);
            return layoutName;
        }

        public static string ViewModel(this IEnumerable<Span> chunks)
        {
            return chunks.OfType<ModelSpan>().FirstValue(x => x.ModelTypeName);
        }


        public static IEnumerable<string> Namespaces(this IEnumerable<Span> chunks)
        {
            return chunks.OfType<NamespaceImportSpan>().Select(x => x.Namespace);
        }
    }
}