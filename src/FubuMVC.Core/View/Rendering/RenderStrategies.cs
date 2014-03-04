using FubuCore.Binding;

namespace FubuMVC.Core.View.Rendering
{
    public class DefaultRenderStrategy : IRenderStrategy
    {
        public bool Applies() { return true; }
        public void Invoke(IRenderAction action) { action.Render(); }
    }

    public class NestedRenderStrategy : IRenderStrategy
    {
        private readonly NestedOutput _nestedOutput;
        public NestedRenderStrategy(NestedOutput nestedOutput)
        {
		    _nestedOutput = nestedOutput;
        }

        public bool Applies()
        {
            return _nestedOutput.IsActive();
        }

        public void Invoke(IRenderAction action)
        {
            action.RenderPartial();
        }
    }

    public class AjaxRenderStrategy : IRenderStrategy
    {
        private readonly IRequestData _requestData;		
        public AjaxRenderStrategy(IRequestData requestData)
        {
            _requestData = requestData;
        }

        public bool Applies()
        {
            return _requestData.IsAjaxRequest();
        }

        public void Invoke(IRenderAction action)
        {
            action.RenderPartial();
        }
    }
}