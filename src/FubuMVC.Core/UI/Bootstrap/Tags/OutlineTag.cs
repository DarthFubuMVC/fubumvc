using FubuCore;
using HtmlTags;

namespace FubuMVC.Core.UI.Bootstrap.Tags
{
    public class OutlineTag : HtmlTag
    {
        public OutlineTag() : base("ul")
        {
            AddClasses("nav", "nav-list");
        }

        public HtmlTag AddHeader(string header)
        {
            return Add("li").AddClass("nav-header").Text(header);
        }

        public OutlineNodeTag AddNode(string label, string anchor = null)
        {
            var tag = new OutlineNodeTag(label, anchor);
            Append(tag);

            return tag;
        }
    }

    public class OutlineNodeTag : HtmlTag
    {
        private readonly HtmlTag _container;

        public OutlineNodeTag(string label, string anchor = null) : base("li")
        {
            _container = Add("div").AddClass("node-container");

            if (anchor.IsEmpty())
            {
                _container.Add("span").Text(label);
            }
            else
            {
                _container.Add("a").Attr("href", "#" + anchor).Text(label);
            }
        }

        public HtmlTag Container
        {
            get { return _container; }
        }
    }
}