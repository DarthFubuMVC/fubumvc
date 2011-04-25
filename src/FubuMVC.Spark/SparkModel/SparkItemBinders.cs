using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;

namespace FubuMVC.Spark.SparkModel
{
    public class BindContext
    {
        public IEnumerable<string> Namespaces { get; set; }
        public string Master { get; set; }
        public string ViewModelType { get; set; }

        public TypePool TypePool { get; set; }
        public SparkItems SparkItems { get; set; }
    }

    public interface ISparkItemBinder
    {
        void Bind(SparkItem item, BindContext context);
    }

    public class MasterPageBinder : ISparkItemBinder
    {
        // Allow for convention on this - consider possibility for other "shared" folders
        private const string DefaultMaster = "Application";

        public void Bind(SparkItem item, BindContext context)
        {
            var masterName = context.Master ?? DefaultMaster;                        
            if (masterName.IsEmpty()) return;

            var locator = new SharedItemLocator(context.SparkItems, new[] {Constants.SharedSpark});
            item.Master = locator.LocateSpark(masterName, item);

            if (item.Master == null)
            {
                // Log -> Spark compiler is about to blow up. // context.Observer.??
            }
        }
    }
    public class ViewModelBinder : ISparkItemBinder
    {
        public void Bind(SparkItem item, BindContext context)
        {
            var fullTypeName = context.ViewModelType;                        
            var matchingTypes = context.TypePool.TypesWithFullName(fullTypeName);
            var type = matchingTypes.Count() == 1 ? matchingTypes.First() : null;

            // Log ambiguity or return "potential types" ?
            item.ViewModelType = type;
        }
    }


    public class NamespaceBinder : ISparkItemBinder
    {
        public void Bind(SparkItem item, BindContext context)
        {
            if (!item.HasViewModel()) return;
            
            var relativePath = item.RelativePath();
            var relativeNamespace = Path.GetDirectoryName(relativePath);

            var nspace = item.ViewModelType.Assembly.GetName().Name;
            if (relativeNamespace.IsNotEmpty())
            {
                nspace += "." + relativeNamespace.Replace(Path.DirectorySeparatorChar, '.');
            }
            
            item.Namespace = nspace;
        }
    }
    public class ViewPathBinder : ISparkItemBinder
    {
        private readonly Cache<string, string> _cache;
        public ViewPathBinder()
        {
            _cache = new Cache<string, string>(getPrefix);
        }

        public void Bind(SparkItem item, BindContext context)
        {
            item.ViewPath = FileSystem.Combine(_cache[item.Origin], item.RelativePath());
        }

        private static string getPrefix(string origin)
        {
            return origin == Constants.HostOrigin ? string.Empty : "__" + origin;
        }
    }
}