﻿using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.View.Model;
using Spark;
using Spark.Compiler;
using Spark.FileSystem;
using Spark.Parser;
using Spark.Parser.Syntax;

namespace FubuMVC.Spark.SparkModel
{
    public interface IChunkLoader
    {
        IEnumerable<Chunk> Load(ITemplate template);
    }

    public class ChunkLoader : IChunkLoader
    {
        private readonly Func<string, IViewFolder> _viewFolder;
        private readonly Cache<string, ViewLoader> _loaders;
        private readonly ISparkSyntaxProvider _syntaxProvider;


        public ChunkLoader() : this(path => new FileSystemViewFolder(path)) { }
        public ChunkLoader(Func<string, IViewFolder> viewFolder)
        {
            _viewFolder = viewFolder;
            _syntaxProvider = new DefaultSyntaxProvider(ParserSettings.DefaultBehavior);

            _loaders = new Cache<string, ViewLoader>(defaultLoaderByRoot);
        }

        public IEnumerable<Chunk> Load(ITemplate template)
        {
            return _loaders[template.RootPath].Load(template.RelativePath()).ToList();
        }

        private ViewLoader defaultLoaderByRoot(string root)
        {
            return new ViewLoader
            {
                SyntaxProvider = _syntaxProvider,
                ViewFolder = _viewFolder(root),
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