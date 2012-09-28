using FubuHtml.Elements;
using HtmlTags;
using HtmlTags.Conventions;

namespace FubuHtml.Security
{
    public class DegradingAccessElementBuilder : ITagBuilder<ElementRequest>
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            var rights = request.AccessRights();
            if (rights.Write) return null;

            // The second span won't be rendered
            return rights.Read 
                       ? request.BuildForCategory(ElementConstants.Display) 
                       : new HtmlTag("span").Authorized(false);
        }

        
    }
}