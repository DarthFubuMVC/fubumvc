using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Spark.Rendering;

namespace FubuMVC.Spark
{
    public class RenderSparkViewBehavior : BasicBehavior
    {
        private readonly ISparkViewRenderer _renderer;

        public RenderSparkViewBehavior(ISparkViewRenderer renderer)
            : base(PartialBehavior.Executes)
        {
            _renderer = renderer;
        }
        protected override DoNext performInvoke()
        {
            _renderer.Render();
            return DoNext.Continue;
        }

    }
}