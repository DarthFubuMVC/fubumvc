using FubuCore.Binding;
using FubuMVC.Core;

namespace FubuMVC.Spark.Rendering
{
    public interface IRenderStrategy
    {
        bool Applies();
        void Invoke();
    }
    public class NestedRenderStrategy : IRenderStrategy
    {
        private readonly NestedOutput _nestedOutput;
        private readonly IViewRenderer _viewRenderer;

        public NestedRenderStrategy(NestedOutput nestedOutput, IViewRenderer viewRenderer)
        {
            _viewRenderer = viewRenderer;
            _nestedOutput = nestedOutput;
        }

        public bool Applies()
        {
            return _nestedOutput.IsActive();
        }

        public void Invoke()
        {
            _viewRenderer.Render();
        }
    }
    public class AjaxRenderStrategy : IRenderStrategy
    {
        private readonly IViewRenderer _viewRenderer;
        private readonly IRequestData _requestData;

        public AjaxRenderStrategy(IViewRenderer viewRenderer, IRequestData requestData)
        {
            _viewRenderer = viewRenderer;
            _requestData = requestData;
        }

        public bool Applies()
        {
            return _requestData.IsAjaxRequest();
        }

        public void Invoke()
        {
            _viewRenderer.Render();
        }
    }
    public class DefaultRenderStrategy : IRenderStrategy
    {
        private readonly IViewRenderer _viewRenderer;

        public DefaultRenderStrategy(IViewRenderer viewRenderer)
        {
            _viewRenderer = viewRenderer;
        }

        public bool Applies()
        {
            return true;
        }

        public void Invoke()
        {
            _viewRenderer.Render();
        }
    }
}