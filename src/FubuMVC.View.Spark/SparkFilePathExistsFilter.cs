using System.Collections.Generic;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark.FileSystem;

namespace FubuMVC.View.Spark
{
    public static class FubuSparkRegistryConfigurationExtensions
    {
        public static void to_spark_view_by_action_namespace_and_name(this ViewsForActionFilterExpression expression, string baseNamespace)
        {
            expression.by(new SparkViewPathInferredFromActionNamespaceAndName(baseNamespace));
        }
    }

    public abstract class SparkFilePathExistsFilter : IViewsForActionFilter
    {
        public abstract string GetSparkViewPath(ActionCall call);
        public abstract string GetSparkViewFile(ActionCall call);

        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            var proposedFolder = GetSparkViewPath(call);
            var proposedFile = GetSparkViewFile(call);

            var sparkFolder = new VirtualPathProviderViewFolder(proposedFolder);

            return ( sparkFolder.HasView(proposedFile))
                 ? new IViewToken[]{new SparkViewToken(proposedFolder + "/" + proposedFile)}
                 : new IViewToken[0];
        }
    }

    public class SparkViewPathInferredFromActionNamespaceAndName : SparkFilePathExistsFilter
    {
        private readonly string _baseNamespace;

        public SparkViewPathInferredFromActionNamespaceAndName(string baseNamespace)
        {
            _baseNamespace = baseNamespace;
        }

        public override string GetSparkViewPath(ActionCall call)
        {
            var root = call.HandlerType.Namespace.Replace(_baseNamespace, "").Replace(".", "/");

            return "~" + root;
        }

        public override string GetSparkViewFile(ActionCall call)
        {
            return call.Method.Name.ToLowerInvariant() + ".spark";
        }
    }
}
