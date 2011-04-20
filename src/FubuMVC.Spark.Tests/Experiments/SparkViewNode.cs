using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Parsing;

namespace FubuMVC.Spark.Tests.Experiments
{
    public class SparkViewNode : OutputNode<RenderSparkViewBehavior>, IMayHaveInputType
    {
        private readonly SparkItem _item;
        private readonly ActionCall _call;

        public SparkViewNode(SparkItem item, ActionCall call)
        {
            _item = item;
            _call = call;
        }

        protected override void configureObject(ObjectDef def)
        {
            var extractor = new ElementNodeExtractor();
            var output = new SparkViewOutput(_item, extractor, new FileSystem());
            def.DependencyByValue(output);
        }
        
        public Type InputType()
        {
            return _call.HasInput ? _call.InputType() : null;
        }
    }
}