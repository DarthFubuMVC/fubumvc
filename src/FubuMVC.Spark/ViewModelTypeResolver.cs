using System;
using System.Linq;
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
        private readonly IViewModelTypeExtractor _viewModelTypeExtractor;
        private readonly Cache<string, Type> _cache;

        public ViewModelTypeResolver(IFileSystem fileSystem, IViewModelTypeExtractor viewModelTypeExtractor)
        {
            _fileSystem = fileSystem;
            _viewModelTypeExtractor = viewModelTypeExtractor;
            _cache = new Cache<string, Type>(resolve);
        }

        public Type Resolve(SparkFile sparkFile)
        {
            return _cache[sparkFile.Path];
        }

        private Type resolve(string path)
        {
            var fileContent = _fileSystem.ReadStringFromFile(path);
            return _viewModelTypeExtractor.Extract(fileContent);            
        }
    }

    public interface IViewModelTypeExtractor
    {
        Type Extract(string templateContent);
    }

    public class ViewModelTypeExtractor : IViewModelTypeExtractor
    {
        private readonly IElementNodeExtractor _nodeExtractor;
        public ViewModelTypeExtractor(IElementNodeExtractor nodeExtractor)
        {
            _nodeExtractor = nodeExtractor;
        }

        public Type Extract(string templateContent)
        {
            var typeName = extractTypeName(templateContent);
            return tryGetType(typeName);
        }

        private Type tryGetType(string fullName)
        {
            return fullName.IsEmpty() ? null : Type.GetType(fullName, false);
        }

        private string extractTypeName(string content)
        {
            return _nodeExtractor
                .ExtractByName(content, "viewdata")
                .Select(n => n.AttributeByName("model"))
                .Where(v => v != null)
                .FirstOrDefault();
        }
    }
}