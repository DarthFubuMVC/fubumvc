using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization.Parsing;

namespace FubuMVC.Spark.Tokenization
{
    public class EnrichmentContext
    {
        public string FileContent { get; set; }
        public TypePool TypePool { get; set; }
        public SparkItems SparkItems { get; set; }        
    }

    public interface ISparkItemModifier
    {
        void Modify(SparkItem item, EnrichmentContext context);
    }

    public class MasterPageModifier : ISparkItemModifier
    {
        // Allow for convention on this - consider possibility for other "shared" folders
        private const string SharedFolder = "Shared";
        private readonly ISparkParser _sparkParser;

        public MasterPageModifier() : this(new SparkParser()) {}
        public MasterPageModifier(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        public void Modify(SparkItem item, EnrichmentContext context)
        {
            var masterName = _sparkParser.ParseMasterName(context.FileContent);
            if (masterName.IsEmpty()) return;            
            
            item.Master = findClosestMaster(masterName, item, context.SparkItems);
            if (item.Master == null)
            {
                // Log -> Spark compiler is about to blow up. // context.Graph.Observer.??
            }
        }

        private SparkItem findClosestMaster(string masterName, SparkItem item, IEnumerable<SparkItem> items)
        {
            var root =  items.Min(x => x.Root);
            var masterLocations = reachableMasterLocations(item.Path, root);
            
            return items
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

    public class ViewModelModifier : ISparkItemModifier
    {
        private readonly ISparkParser _sparkParser;

        public ViewModelModifier() : this(new SparkParser()) {}
        public ViewModelModifier(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        // Log ambiguity or return "potential types" ?
        // context.Graph.Observer.??
        public void Modify(SparkItem item, EnrichmentContext context)
        {
            var fullTypeName = _sparkParser.ParseViewModelTypeName(context.FileContent);
            var matchingTypes = context.TypePool.TypesWithFullName(fullTypeName);
            var type = matchingTypes.Count() == 1 ? matchingTypes.First() : null;

            item.ViewModelType = type;
        }
    }

    public class NamespaceModifier : ISparkItemModifier
    {
        public void Modify(SparkItem item, EnrichmentContext context)
        {
            if (!item.HasViewModel()) return;
            
            item.Namespace = resolveNamespace(item);            
        }

        // TODO : Get opinions on this.
        private static string resolveNamespace(SparkItem item)
        {
            var relativePath = item.RelativePath();
            var relativeNamespace = Path.GetDirectoryName(relativePath);

            var nspace = item.ViewModelType.Assembly.GetName().Name;
            if (relativeNamespace.IsNotEmpty())
            {
                nspace += "." + relativeNamespace.Replace(Path.DirectorySeparatorChar, '.');
            }

            return nspace;
        }
    }
}