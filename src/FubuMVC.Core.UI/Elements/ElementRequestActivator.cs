using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.UI.Elements
{
    public class ElementRequestActivator : TagRequestActivator<ElementRequest> 
    {
        private readonly IFubuRequest _request;
        private readonly IElementNamingConvention _naming;

        public ElementRequestActivator(IFubuRequest request, IElementNamingConvention naming)
        {
            _request = request;
            _naming = naming;
        }

        public override void Activate(ElementRequest request)
        {
            request.ElementId = _naming.GetName(request.HolderType(), request.Accessor);
            if (request.Model == null)
            {
                request.Model = _request.Get(request.HolderType());
            }
        }
    }
}