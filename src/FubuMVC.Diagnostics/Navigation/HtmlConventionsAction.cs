using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Models.Html;

namespace FubuMVC.Diagnostics.Navigation
{
    public class HtmlConventionsAction : NavigationItemBase
    {
        public HtmlConventionsAction(BehaviorGraph graph, IEndpointService endpointService)
            : base(graph, endpointService)
        {
        }

        public override string Text()
        {
            return "Html Conventions";
        }

        protected override object inputModel()
        {
            return new HtmlConventionsRequestModel();
        }
    }
}