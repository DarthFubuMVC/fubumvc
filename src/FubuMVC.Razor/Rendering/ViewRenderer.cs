using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Razor.Rendering
{
    public interface IViewRenderer
    {
        void Render();
        void RenderPartial();
    }

    public class ViewRenderer : IViewRenderer
    {
        private readonly IEnumerable<IRenderStrategy> _strategies;
        private readonly IRenderAction _renderAction;

        public ViewRenderer(IEnumerable<IRenderStrategy> strategies, IRenderAction renderAction)
        {
            _strategies = strategies;
            _renderAction = renderAction;
        }

        public void Render()
        {
            _strategies.First(x => x.Applies()).Invoke(_renderAction);
        }

        public void RenderPartial()
        {
            _strategies.First(x => x.Applies()).InvokePartial(_renderAction);
        }
    }
}