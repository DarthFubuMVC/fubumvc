using FubuMVC.Core;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Spark.Tokenization.Nodes
{
    public class SparkViewRenderer : BasicBehavior
    {
        public SparkViewRenderer()
            : base(PartialBehavior.Executes)
        {
        }
        protected override DoNext performInvoke()
        {
            return DoNext.Continue;
        }
    }
}