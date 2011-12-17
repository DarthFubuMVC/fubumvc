using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor
{
    public class RenderRazorBehavior : BasicBehavior
    {
        private readonly IViewRenderer _renderer;
        public RenderRazorBehavior(IViewRenderer renderer)
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