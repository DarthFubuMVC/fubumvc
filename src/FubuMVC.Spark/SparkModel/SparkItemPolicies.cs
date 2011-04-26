using System.IO;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkItemPolicy
    {
        bool Matches(SparkItem item);
        void Apply(SparkItem item);
    }

    public class NamespacePolicy : ISparkItemPolicy
    {
        public bool Matches(SparkItem item)
        {
            return item.HasViewModel() && item.Namespace.IsEmpty();
        }

        public void Apply(SparkItem item)
        {
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

    public class ViewPathPolicy : ISparkItemPolicy
    {
        private readonly Cache<string, string> _cache;
        public ViewPathPolicy()
        {
            _cache = new Cache<string, string>(getPrefix);
        }

        public bool Matches(SparkItem item)
        {
            return item.ViewPath.IsEmpty();
        }

        public void Apply(SparkItem item)
        {
            item.ViewPath = FileSystem.Combine(_cache[item.Origin], item.RelativePath());
        }

        private static string getPrefix(string origin)
        {
            return origin == Constants.HostOrigin ? string.Empty : "__" + origin;
        }
    }
}