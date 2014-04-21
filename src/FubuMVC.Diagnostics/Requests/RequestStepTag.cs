using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.UI.Bootstrap.Tags;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
    public class RequestStepTag : HtmlTag
    {
        public RequestStepTag(RequestStep step, string content) : base("div")
        {
            AddClass("row-fluid");
            Id(step.Id.ToString());

            Text(content).Encoded(false);

            this.PrependAnchor();
        }
    }
}