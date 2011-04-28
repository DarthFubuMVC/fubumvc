using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Spark.Rendering;

namespace FubuMVC.Spark
{
    public class RenderSparkBehavior : BasicBehavior
    {
        private readonly ISparkViewRenderer _renderer;
        public RenderSparkBehavior(ISparkViewRenderer renderer) : 
            base(PartialBehavior.Executes)
        {
            _renderer = renderer;
        }

        protected override DoNext performInvoke()
        {
            _renderer.Render();
            // Get Func<Stream>?
            return DoNext.Continue;
        }

        protected override void afterInsideBehavior()
        {
            // Flush stream?
            // Or avoid this and perhaps a _renderer.Render(Callback when ready to flush?)
        }
    }
}