using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Parsing;

namespace FubuMVC.Spark.Tests.Experiments
{
    public class SparkViewNode : OutputNode<RenderSparkViewBehavior>, IMayHaveInputType
    {
        private readonly SparkFile _file;
        private readonly ActionCall _call;

        public SparkViewNode(SparkFile file, ActionCall call)
        {
            _file = file;
            _call = call;
        }

        protected override void configureObject(ObjectDef def)
        {
            var extractor = new ElementNodeExtractor();
            var output = new SparkViewOutput(_file, extractor, new FileSystem());
            def.DependencyByValue(output);
        }
        
        public Type InputType()
        {
            return _call.HasInput ? _call.InputType() : null;
        }
    }
}