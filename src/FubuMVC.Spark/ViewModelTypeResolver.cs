using System;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Spark.Parsing;
using FubuMVC.Spark.Scanning;

// TODO : UT

namespace FubuMVC.Spark
{
    public interface IViewModelTypeResolver
    {
        Type Resolve(SparkFile sparkFile);
    }

    public class ViewModelTypeResolver : IViewModelTypeResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly IViewModelTypeParser _viewModelTypeParser;
        private readonly Cache<string, Type> _cache;

        public ViewModelTypeResolver(IFileSystem fileSystem, IViewModelTypeParser viewModelTypeParser)
        {
            _fileSystem = fileSystem;
            _viewModelTypeParser = viewModelTypeParser;
            _cache = new Cache<string, Type>(resolve);
        }

        public Type Resolve(SparkFile sparkFile)
        {
            return _cache[sparkFile.Path];
        }

        private Type resolve(string path)
        {
            var fileContent = _fileSystem.ReadStringFromFile(path);
            return _viewModelTypeParser.Parse(fileContent);            
        }
    }
}