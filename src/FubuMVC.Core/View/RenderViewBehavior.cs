using FubuMVC.Core.Behaviors;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View
{
    public class RenderViewBehavior : BasicBehavior
    {
        private readonly IViewRenderer _renderer;
        public RenderViewBehavior(IViewRenderer renderer)
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