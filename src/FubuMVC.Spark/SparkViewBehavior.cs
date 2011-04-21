using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Spark.Rendering;

namespace FubuMVC.Spark
{
    public class SparkViewBehavior : BasicBehavior
    {
        // Fields
        private readonly IEnumerable<IRenderAction> _actions;

        // Methods
        public SparkViewBehavior(IEnumerable<IRenderAction> actions) : base(PartialBehavior.Executes)
        {
            _actions = actions;
        }

        protected override DoNext performInvoke()
        {
            var renderContext = new RenderContext();
            _actions.Each(pipe => pipe.Invoke(renderContext));
            return DoNext.Continue;
        }
    }
}