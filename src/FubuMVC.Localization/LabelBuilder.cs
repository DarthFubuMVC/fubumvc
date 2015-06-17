using FubuLocalization;
using FubuMVC.Core.UI.Elements;
using HtmlTags;

namespace FubuMVC.Localization
{
    public class LabelBuilder : ElementTagBuilder
    {
    	public override bool Matches(ElementRequest subject)
    	{
    		return true;
    	}

    	public override HtmlTag Build(ElementRequest request)
        {
            string header = LocalizationManager.GetHeader(request.Accessor.InnerProperty);
            return new HtmlTag("label").Attr("for", request.ElementId).Text(header);
        }
    }
}