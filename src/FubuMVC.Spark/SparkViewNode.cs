using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Spark.Scanning;

namespace FubuMVC.Spark
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

        public Type InputType()
        {
            throw new NotImplementedException();
        }
    }
}