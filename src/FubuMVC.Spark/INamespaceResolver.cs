using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Scanning;

namespace FubuMVC.Spark
{
    public interface INamespaceResolver
    {
        string Resolve(SparkFile sparkSparkFile);
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