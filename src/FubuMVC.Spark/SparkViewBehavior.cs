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
            // Do we really wish the runtime model to know about all of this? What happened to usage of Func?
            // Does our runtime model need to be that coupled to our spark model? 
            // Makes it harder to make changes in one place without causing ripples.

            var renderContext = new RenderContext();
            _actions.Each(pipe => pipe.Invoke(renderContext));
            return DoNext.Continue;
        }
    }
}