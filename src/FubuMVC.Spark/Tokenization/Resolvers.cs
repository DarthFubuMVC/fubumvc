using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Parsing;

// TODO : UT

namespace FubuMVC.Spark.Tokenization
{
    public interface IViewModelTypeResolver
    {
        Type Resolve(SparkFile sparkFile);
    }
    public class ViewModelTypeResolver : IViewModelTypeResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISparkParser _sparkParser;
        private readonly TypePool _typePool;
        private readonly Cache<string, Type> _cache;

        public ViewModelTypeResolver(IFileSystem fileSystem, ISparkParser sparkParser, TypePool typePool)
        {
            _fileSystem = fileSystem;
            _sparkParser = sparkParser;
            _typePool = typePool;

            _cache = new Cache<string, Type>(resolve);
        }

        public Type Resolve(SparkFile sparkFile)
        {
            return _cache[sparkFile.Path];
        }

        private Type resolve(string path)
        {
            var fileContent = _fileSystem.ReadStringFromFile(path);
            var fullTypeName = _sparkParser.ParseViewModelTypeName(fileContent);

            // Log ambiguity or return "potential types" ?
            var matchingTypes = _typePool.TypesWithFullName(fullTypeName);
            return matchingTypes.Count() == 1 ? matchingTypes.First() : null;
        }
    }

    public interface INamespaceResolver
    {
        string Resolve(SparkFile sparkFile);
    }
    public class NamespaceResolver : INamespaceResolver
    {
        public string Resolve(SparkFile sparkFile)
        {
            //TODO: FIX THIS, INTRODUCE PROPER ALGORITHM
            if (sparkFile.ViewModel == null)
            {
                return null;
            }
            var ns = sparkFile.ViewModel.Assembly.GetName().Name;

            var relativePath = sparkFile.Path.PathRelativeTo(sparkFile.Root);
            var relativeNamespace = Path.GetDirectoryName(relativePath).Replace(Path.DirectorySeparatorChar, '.');

            if (relativeNamespace.Length > 0)
            {
                ns += "." + relativeNamespace;
            }

            return ns;
        }
    }
}