using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Runtime.Files;
using Spark;
using Spark.Compiler;
using Spark.FileSystem;
using Spark.Parser;
using Spark.Parser.Syntax;

namespace FubuMVC.Spark.SparkModel
{
    public interface IChunkLoader
    {
        IEnumerable<Chunk> Load(ISparkTemplate template, IFubuApplicationFiles files);
    }

    public class ChunkLoader : IChunkLoader
    {
        private readonly Func<string, IViewFolder> _viewFolder;
        private readonly Cache<string, ViewLoader> _loaders;
        private readonly ISparkSyntaxProvider _syntaxProvider;


        public ChunkLoader()
        {
            _viewFolder = path => new FileSystemViewFolder(path);
            _syntaxProvider = new DefaultSyntaxProvider(ParserSettings.DefaultBehavior);

            _loaders = new Cache<string, ViewLoader>(defaultLoaderByRoot);
        }

        public IEnumerable<Chunk> Load(ISparkTemplate template, IFubuApplicationFiles files)
        {
            if (template.RelativePath().IsEmpty())
            {
                throw new ArgumentOutOfRangeException("Invalid template path for file " + template.FilePath);
            }

            try
            {
                var viewLoader = _loaders[files.RootPath];
                var chunks = viewLoader.Load(template.RelativePath());
                if (chunks == null)
                {
                    throw new Exception("Unable to parse file '{0}'".ToFormat(template.RelativePath()));
                }

                return chunks.ToList();
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("Unable to parse file")) throw;

                throw new Exception("Unable to parse file '{0}'".ToFormat(template.RelativePath()), e);
            }
        }

        private ViewLoader defaultLoaderByRoot(string root)
        {
            return new ViewLoader
            {
                SyntaxProvider = _syntaxProvider,
                ViewFolder = _viewFolder(root)
            };
        }
    }

    public static class ChunkEnumerableExtensions
    {
        public static string Master(this IEnumerable<Chunk> chunks)
        {
            return chunks.OfType<UseMasterChunk>().FirstValue(x => x.Name);
        }

        public static string ViewModel(this IEnumerable<Chunk> chunks)
        {
            return chunks.OfType<ViewDataModelChunk>().FirstValue(x => x.TModel);
        }

        public static IEnumerable<string> Namespaces(this IEnumerable<Chunk> chunks)
        {
            return chunks.OfType<UseNamespaceChunk>().Select(x => x.Namespace.ToString());
        }
    }
}