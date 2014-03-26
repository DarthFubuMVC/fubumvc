using HtmlTags;

namespace FubuMVC.Core.UI.Bootstrap.Collapsibles
{
    public class CollapsibleTag : HtmlTag
    {
        private readonly HtmlTag _body;

        public CollapsibleTag(string id, string title) : base("div")
        {
            AddClass("accordion-group");
            Id(id);

            var bodyId = id + "-body";

            Add("div").AddClass("accordion-heading").Add("a")
                .AddClass("accordion-toggle")
                .Data("toggle", "collapse")
                .Attr("href", "#" + bodyId)
                .Text(title);

            _body = Add("div").Id(bodyId).AddClasses("accordion-body", "collapse")
                .Add("div").AddClass("accordion-inner");
        }

        public CollapsibleTag Opened(bool isOpen)
        {
            if (isOpen)
            {
                AddClass("in");
            }

            return this;
        }

        public bool Opened()
        {
            return HasClass("in");
        }

        public CollapsibleTag AppendContent(HtmlTag tag)
        {
            _body.Append(tag);

            return this;
        }

        /// <summary>
        /// Adds literal Html to the body of this CollapsibleTag
        /// </summary>
        /// <param name="html"></param>
        public void AppendContent(string html)
        {
            _body.AppendHtml(html);
        }
    }
}