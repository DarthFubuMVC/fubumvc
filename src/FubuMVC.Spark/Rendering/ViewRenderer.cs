using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewRenderer
    {
        void Render();
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
    }
}