using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Razor
{
    public class RenderRazorBehavior : IActionBehavior
    {
        private readonly IViewRenderer _renderer;

        public RenderRazorBehavior(IViewRenderer renderer)
        {
            _renderer = renderer;
        }

        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            _renderer.Render();
            if(InsideBehavior != null)
                InsideBehavior.Invoke();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
            //_renderer.RenderPartial();
            //if(InsideBehavior != null)
            //    InsideBehavior.InvokePartial();
        }
    }
}