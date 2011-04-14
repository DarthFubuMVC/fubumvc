using System;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Parsing;

namespace FubuMVC.Spark.Tokenization
{
    public class EnrichmentContext
    {
        public string FileContent { get; set; }
        public TypePool TypePool { get; set; }
        public SparkFiles SparkFiles { get; set; }
    }

    public interface ISparkFileEnricher
    {
        void Enrich(SparkFile file, EnrichmentContext context);
    }

    public class MasterPageEnricher : ISparkFileEnricher
    {
        private readonly ISparkParser _sparkParser;
        public MasterPageEnricher(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        public void Enrich(SparkFile file, EnrichmentContext context)
        {
            var masterName = _sparkParser.ParseMasterName(context.FileContent);
            var masterPath = masterName.IsNotEmpty() 
                ? findMaster(masterName, file, context.SparkFiles) 
                : null;

            file.MasterPath = masterPath;
        }

        // Extract later // Get Shared folder names injected
        private string findMaster(string masterName, SparkFile file, SparkFiles sparkFiles)
        {
            // closest reachable ancestor "shared" directory with a file named masterName
            return null;
        }
    }

    public class NamespaceEnricher : ISparkFileEnricher
    {
        public void Enrich(SparkFile file, EnrichmentContext context)
        {
            file.Namespace = resolveNamespace(file.ViewModelType, file.Root, file.Path);            
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
        private readonly ISparkParser _sparkParser;
        public ViewModelEnricher(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        // Log ambiguity or return "potential types" ?
        public void Enrich(SparkFile file, EnrichmentContext context)
        {
            var fullTypeName = _sparkParser.ParseViewModelTypeName(context.FileContent);
            var matchingTypes = context.TypePool.TypesWithFullName(fullTypeName);
            var type = matchingTypes.Count() == 1 ? matchingTypes.First() : null;

            file.ViewModelType = type;
        }
    }
}