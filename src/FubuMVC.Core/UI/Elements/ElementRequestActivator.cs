using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.UI.Elements
{
    public class ElementRequestActivator : ElementIdActivator
    {
        private readonly IFubuRequest _request;

        public ElementRequestActivator(IFubuRequest request, IElementNamingConvention naming)
            : base(naming)
        {
            _request = request;
        }

        public override void Activate(ElementRequest request)
        {
            base.Activate(request);
            if (request.Model == null)
            {
                request.Model = _request.Get(request.HolderType());
            }
        }
    }
}