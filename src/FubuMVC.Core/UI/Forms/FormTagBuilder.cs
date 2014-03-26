using HtmlTags;
using HtmlTags.Conventions;

namespace FubuMVC.Core.UI.Forms
{
    public class FormTagBuilder : TagBuilder<FormRequest>
    {
        public override bool Matches(FormRequest subject)
        {
            return true;
        }

        public override HtmlTag Build(FormRequest request)
        {
            var tag = new FormTag(request.Url);

            if (!request.CloseTag)
            {
                tag.NoClosingTag();
            }

            return tag;
        }
    }
}