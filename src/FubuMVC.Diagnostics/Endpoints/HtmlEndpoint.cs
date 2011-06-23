using FubuMVC.Diagnostics.Models.Html;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class HtmlEndpoint
    {
        public HtmlConventionsViewModel Get(HtmlConventionsRequestModel request)
        {
            return new HtmlConventionsViewModel();
        }
    }
}