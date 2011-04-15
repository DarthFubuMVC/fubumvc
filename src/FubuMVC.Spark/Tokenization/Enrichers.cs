using System.Collections.Generic;
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

    // TODO: Order of execution matters for these enrichers - see if we can break away from that.

    public interface ISparkFileEnricher
    {
        void Enrich(SparkFile file, EnrichmentContext context);
    }

    public class MasterPageEnricher : ISparkFileEnricher
    {
        // Allow for convention on this - consider possibility for other "shared" folders
        private const string SharedFolder = "Shared";
        private readonly ISparkParser _sparkParser;

        public MasterPageEnricher() : this(new SparkParser()) {}
        public MasterPageEnricher(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        public void Enrich(SparkFile file, EnrichmentContext context)
        {
            var masterName = _sparkParser.ParseMasterName(context.FileContent);
            if (masterName.IsEmpty()) return;            
            
            file.Master = findClosestMaster(masterName, file, context.SparkFiles);
            if (file.Master == null)
            {
                // Log -> Spark compiler is about to blow up. // context.Graph.Observer.??
            }
        }

        private SparkFile findClosestMaster(string masterName, SparkFile file, IEnumerable<SparkFile> files)
        {
            var root =  files.Min(x => x.Root);
            var masterLocations = reachableMasterLocations(file.Path, root);
            
            return files
                .Where(x => x.Name() == masterName)
                .Where(x => masterLocations.Contains(x.DirectoryPath()))
                .FirstOrDefault();
        }
        private IEnumerable<string> reachableMasterLocations(string path, string root)
        {
            do
            {
                path = Path.GetDirectoryName(path);
                if (path == null) break;      
                // TODO : Consider yield return path - if we should look in each ancestor folder
                yield return Path.Combine(path, SharedFolder);

            } while (path.IsNotEmpty() && path.PathRelativeTo(root).IsNotEmpty());
        }
    }

    public class ViewModelEnricher : ISparkFileEnricher
    {
        private readonly ISparkParser _sparkParser;

        public ViewModelEnricher() : this(new SparkParser()) {}
        public ViewModelEnricher(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        // Log ambiguity or return "potential types" ?
        // context.Graph.Observer.??
        public void Enrich(SparkFile file, EnrichmentContext context)
        {
            var fullTypeName = _sparkParser.ParseViewModelTypeName(context.FileContent);
            var matchingTypes = context.TypePool.TypesWithFullName(fullTypeName);
            var type = matchingTypes.Count() == 1 ? matchingTypes.First() : null;

            file.ViewModelType = type;
        }
    }

    public class NamespaceEnricher : ISparkFileEnricher
    {
        public void Enrich(SparkFile file, EnrichmentContext context)
        {
            if (!file.HasViewModel()) return;
            
            file.Namespace = resolveNamespace(file);            
        }

        // TODO : Get opinions on this.
        private static string resolveNamespace(SparkFile file)
        {
            var relativePath = file.RelativePath();
            var relativeNamespace = Path.GetDirectoryName(relativePath);

            var nspace = file.ViewModelType.Assembly.GetName().Name;
            if (relativeNamespace.IsNotEmpty())
            {
                nspace += "." + relativeNamespace.Replace(Path.DirectorySeparatorChar, '.');
            }

            return nspace;
        }
    }
}