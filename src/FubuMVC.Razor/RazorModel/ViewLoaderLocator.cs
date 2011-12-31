using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Parser.SyntaxTree;
using FubuCore.Util;
using FubuMVC.Razor.FileSystem;
using FubuMVC.Razor.RazorEngine.Parsing;
using RazorEngine.Spans;

namespace FubuMVC.Razor.RazorModel
{
    public interface IViewLoaderLocator
    {
        IViewLoader Locate(ITemplate template);
    }

    public class ViewLoaderLocator : IViewLoaderLocator
    {
        private readonly Func<string, IViewFolder> _viewFolder;
        private readonly Cache<string, IViewLoader> _loaders;

        public ViewLoaderLocator() : this(path => new FileSystemViewFolder(path)) { }
        public ViewLoaderLocator(Func<string, IViewFolder> viewFolder)
        {
            _viewFolder = viewFolder;

            _loaders = new Cache<string, IViewLoader>(defaultLoaderByRoot);
        }

        public IViewLoader Locate(ITemplate template)
        {
            return _loaders[template.FilePath];
        }

        private ViewLoader defaultLoaderByRoot(string root)
        {
            return new ViewLoader
            {
                ViewFolder = _viewFolder(root),
            };
        }
    }

    public static class ChunkEnumerableExtensions
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