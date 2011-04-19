using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Spark.Tokenization;
using Spark;
using Spark.FileSystem;
using Constants = FubuMVC.Spark.Tokenization.Constants;

namespace FubuMVC.Spark.Tests.Experiments
{
    public interface ISparkEngineFactory
    {
        ISparkViewEngine GetEngine(SparkItem sparkItem);
    }

    public class SparkEngineFactory : ISparkEngineFactory
    {
        private string _rootPath;
        private IEnumerable<string> _packagesRoots;
        private readonly Cache<Tuple<string, string>, ISparkViewEngine> _cache;

        public SparkEngineFactory()
        {
            _cache = new Cache<Tuple<string, string>, ISparkViewEngine>(getEngine);
        }

        private ISparkViewEngine getEngine(Tuple<string, string> key)
        {
            var root = key.Item1;
            var origin = key.Item2;

            // fetch this from somewhere else
            var settings = new SparkSettings();
            var engine = new SparkViewEngine(settings)
            {
                ViewFolder = new FileSystemViewFolder(root)
            };
            if (origin == Constants.HostOrigin)
            {
                foreach (var packageRoot in _packagesRoots)
                {
                    engine.ViewFolder = engine.ViewFolder.Append(new FileSystemViewFolder(packageRoot));
                }
            }
            else
            {
                engine.ViewFolder = engine.ViewFolder.Append(new FileSystemViewFolder(_rootPath));
            }
            return engine;
        }


        // NOTE:TEMP
        public void SetRoot(string rootPath)
        {
            _rootPath = rootPath;
        }

        // NOTE:TEMP
        public void SetPackagesRoots(IEnumerable<string> packagesRoots)
        {
            _packagesRoots = packagesRoots;
        }

        public ISparkViewEngine GetEngine(SparkItem sparkItem)
        {
            var engine = _cache[new Tuple<string, string>(sparkItem.Root, sparkItem.Origin)];
            return engine;
        }
    }
}