using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.SparkModel.Parsing;

namespace FubuMVC.Spark.SparkModel
{
    // Not so happy about this whole "binding", but it somewhat allows for 
    // reversibility because of its central usage. Ask JDM for advice/feedback.

    public class BindContext
    {
        public string FileContent { get; set; }
        public TypePool TypePool { get; set; }
        public SparkItems SparkItems { get; set; }
    }

    // Take a good look at binders in Fubu. Would be nice to use IPropertyBinder etc. But it assumes IoC :/
    public interface ISparkItemBinder
    {
        bool Applies(SparkItem item);
        void Bind(SparkItem item, BindContext context);
    }

    // Extract logic into something less if else smelly - introduce two modifiers to deal with packages and host, respectively.
    public class MasterPageBinder : ISparkItemBinder
    {
        // Allow for convention on this - consider possibility for other "shared" folders
        private const string SharedFolder = "Shared";
        private const string DefaultMaster = "Application";

        private readonly ISparkParser _sparkParser;

        public MasterPageBinder() : this(new SparkParser()) { }
        public MasterPageBinder(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        public bool Applies(SparkItem item)
        {
            return true;
        }

        public void Bind(SparkItem item, BindContext context)
        {
            var masterName = _sparkParser.ParseMasterName(context.FileContent) ?? DefaultMaster;
            if (masterName.IsEmpty()) return;

            // Extract into something similar to https://gist.github.com/929869
            item.Master = findClosestMaster(masterName, item, context.SparkItems);

            if (item.Master == null)
            {
                // Log -> Spark compiler is about to blow up. // context.Observer.??
            }
        }

        private SparkItem findClosestMaster(string masterName, SparkItem item, IEnumerable<SparkItem> items)
        {
            // reconsider this, as a package can be in development mode.
            var root = items.Min(x => x.RootPath);
            var masterLocations = reachableMasterLocations(item.FilePath, root);

            return items
                .Where(x => x.Name() == masterName)
                .Where(x => masterLocations.Contains(x.DirectoryPath()))
                .FirstOrDefault();
        }

        private static IEnumerable<string> reachableMasterLocations(string path, string root)
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

    public class ViewModelBinder : ISparkItemBinder
    {
        private readonly ISparkParser _sparkParser;

        public ViewModelBinder() : this(new SparkParser()) { }
        public ViewModelBinder(ISparkParser sparkParser)
        {
            _sparkParser = sparkParser;
        }

        public bool Applies(SparkItem item)
        {
            return true;
        }

        public void Bind(SparkItem item, BindContext context)
        {
            var fullTypeName = _sparkParser.ParseViewModelTypeName(context.FileContent);
            var matchingTypes = context.TypePool.TypesWithFullName(fullTypeName);
            var type = matchingTypes.Count() == 1 ? matchingTypes.First() : null;

            // Log ambiguity or return "potential types" ?
            // context.Observer.??

            item.ViewModelType = type;
        }
    }

    public class NamespaceBinder : ISparkItemBinder
    {
        public bool Applies(SparkItem item)
        {
            return item.HasViewModel();
        }

        public void Bind(SparkItem item, BindContext context)
        {
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

    public class PathPrefixBinder : ISparkItemBinder
    {
        private readonly Cache<string, string> _cache;
        public PathPrefixBinder()
        {
            _cache = new Cache<string, string>(getPrefix);
        }

        public bool Applies(SparkItem item)
        {
            return true;
        }

        public void Bind(SparkItem item, BindContext context)
        {
            item.PathPrefix = _cache[item.Origin];
        }

        private static string getPrefix(string origin)
        {
            return origin == Constants.HostOrigin ? string.Empty : "__" + origin + "__";
        }
    }
}