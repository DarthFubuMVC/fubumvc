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
    public interface IChunkLoader
    {
        IEnumerable<Span> Load(ITemplate template);
    }

    public class ChunkLoader : IChunkLoader
    {
        private readonly Func<string, IViewFolder> _viewFolder;
        private readonly Cache<string, ViewLoader> _loaders;


        public ChunkLoader() : this(path => new FileSystemViewFolder(path)) { }
        public ChunkLoader(Func<string, IViewFolder> viewFolder)
        {
            _viewFolder = viewFolder;

            _loaders = new Cache<string, ViewLoader>(defaultLoaderByRoot);
        }

        public IEnumerable<Span> Load(ITemplate template)
        {
            return _loaders[template.RootPath].Load(template.RelativePath()).ToList();
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
            //return chunks.OfType<Master>().FirstValue(x => x.Name);
            //TODO maybe add a new type of Span to determine Layout before compiling?
            return null;
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