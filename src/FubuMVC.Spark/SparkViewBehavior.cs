using FubuMVC.Core;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Spark
{
    public class SparkViewBehavior : BasicBehavior
    {
        public SparkViewBehavior() : base(PartialBehavior.Executes)
        {

        }

        protected override DoNext performInvoke()
        {
            return DoNext.Continue;
        }
    }
}