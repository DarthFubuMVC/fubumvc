using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark.FileSystem;

namespace FubuMVC.View.Spark
{
    public static class FubuSparkRegistryConfigurationExtensions
    {
        public static void to_spark_view_by_action_namespace_and_name(this ViewAttachmentStrategyExpression expression, string baseNamespace)
        {
            expression.by(new SparkViewInferredPathFromActionNamespaceAndNameStrategy(baseNamespace));
        }
    }

    public abstract class SparkFilePathAttachmentStrategy : IViewAttachmentStrategy
    {
        public abstract string GetSparkViewPath(ActionCall call);
        public abstract string GetSparkViewFile(ActionCall call);

        public IEnumerable<IViewToken> Find(ActionCall call, ViewBag views)
        {
            var proposedFolder = GetSparkViewPath(call);
            var proposedFile = GetSparkViewFile(call);

            var sparkFolder = new VirtualPathProviderViewFolder(proposedFolder);

            return ( sparkFolder.HasView(proposedFile))
                 ? new IViewToken[]{new SparkViewToken(proposedFolder + "/" + proposedFile)}
                 : new IViewToken[0];
        }
    }

    public class SparkViewInferredPathFromActionNamespaceAndNameStrategy : SparkFilePathAttachmentStrategy
    {
        private readonly string _baseNamespace;

        public SparkViewInferredPathFromActionNamespaceAndNameStrategy(string baseNamespace)
        {
            _baseNamespace = baseNamespace.ToLowerInvariant();
        }

        public override string GetSparkViewPath(ActionCall call)
        {
            var root = call.HandlerType.Namespace.ToLowerInvariant().Replace(_baseNamespace, "").Replace(".", "/");

            return "~" + root;
        }

        public override string GetSparkViewFile(ActionCall call)
        {
            return call.Method.Name.ToLowerInvariant() + ".spark";
        }
    }
}