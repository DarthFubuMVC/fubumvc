using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Parsing;

namespace FubuMVC.Spark.Tokenization
{
    public interface ISparkFileEnricher
    {
        void Alter(SparkFile file);
    }

    public class NamespaceEnricher : ISparkFileEnricher
    {
        public void Alter(SparkFile file)
        {
            var ns = resolveNamespace(file.ViewModel, file.Root, file.Path);
            file.Namespace = ns;
        }
        private static string resolveNamespace(Type viewModelType, string root, string path)
        {
            //TODO: FIX THIS, INTRODUCE PROPER ALGORITHM
            if (viewModelType == null)
            {
                return null;
            }
            var ns = viewModelType.Assembly.GetName().Name;

            var relativePath = path.PathRelativeTo(root);
            var relativeNamespace = Path.GetDirectoryName(relativePath).Replace(Path.DirectorySeparatorChar, '.');

            if (relativeNamespace.Length > 0)
            {
                ns += "." + relativeNamespace;
            }

            return ns;
        }
    }

    public class ViewModelEnricher : ISparkFileEnricher
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISparkParser _sparkParser;
        private readonly TypePool _typePool;

        public ViewModelEnricher(IFileSystem fileSystem, ISparkParser sparkParser, TypePool typePool)
        {
            _fileSystem = fileSystem;
            _sparkParser = sparkParser;
            _typePool = typePool;
        }

        public void Alter(SparkFile file)
        {
            file.ViewModel = resolve(file.Path);
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
}