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
        private readonly IRenderAction _renderAction;
        
		public NestedRenderStrategy(IRenderAction renderAction, NestedOutput nestedOutput)
        {
            _renderAction = renderAction;
            _nestedOutput = nestedOutput;
        }

        public bool Applies()
        {
            return _nestedOutput.IsActive();
        }

        public void Invoke()
        {
            _renderAction.Render();
        }
    }

    public class AjaxRenderStrategy : IRenderStrategy
    {
        private readonly IRenderAction _renderAction;
        private readonly IRequestData _requestData;
		
        public AjaxRenderStrategy(IRenderAction renderAction, IRequestData requestData)
        {
            _renderAction = renderAction;
            _requestData = requestData;
        }

        public bool Applies()
        {
            return _requestData.IsAjaxRequest();
        }

        public void Invoke()
        {
            _renderAction.Render();
        }
    }

    public class DefaultRenderStrategy : IRenderStrategy
    {
        private readonly IRenderAction _renderAction;
        public DefaultRenderStrategy(IRenderAction renderAction)
        {
            _renderAction = renderAction;
        }

        public bool Applies()
        {
            return true;
        }

        public void Invoke()
        {
            _renderAction.Render();
        }
    }
}