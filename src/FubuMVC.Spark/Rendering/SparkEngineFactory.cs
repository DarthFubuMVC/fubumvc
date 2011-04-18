using System.Collections.Generic;
using FubuMVC.Spark.Tokenization;
using Spark;
using Spark.FileSystem;
using Constants = Spark.Constants;

namespace FubuMVC.Spark.Rendering
{
    public interface ISparkEngineFactory
    {
        ISparkViewEngine GetEngine(SparkItem sparkItem);
    }

    public class SparkEngineFactory : ISparkEngineFactory
    {
        private string _rootPath;
        private IEnumerable<string> _packagesRoots;

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
            // fetch this from somewhere else
            var settings = new SparkSettings();
            var engine = new SparkViewEngine(settings)
            {
                ViewFolder = new FileSystemViewFolder(sparkItem.Root)
            };
            if (sparkItem.Origin == Tokenization.Constants.HostOrigin)
            {
                foreach (var root in _packagesRoots)
                {
                    engine.ViewFolder = engine.ViewFolder.Append(new FileSystemViewFolder(root));
                }
            }
            else
            {
                engine.ViewFolder = engine.ViewFolder.Append(new FileSystemViewFolder(_rootPath));
            }
            // not final code, ideally, the engine should be cached by sparkItem.Root (string)
            return engine;
        }
    }
}