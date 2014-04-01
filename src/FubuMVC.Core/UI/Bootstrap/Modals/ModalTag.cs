using HtmlTags;

namespace FubuMVC.Core.UI.Bootstrap.Modals
{
    public class ModalTag : HtmlTag
    {
        private readonly HtmlTag _body;
        private readonly HtmlTag _footer;
        private HtmlTag _label;

        public ModalTag(string id)
            : base("div")
        {
            var labelId = id + "Label";
            var bodyId = id + "Body";

            AddClass("modal");
            AddClass("hide");

            Id(id);
            Attr("tabindex", "-1");
            Attr("role", "dialog");
            Attr("aria-labelledby", labelId);
            Attr("aria-hidden", "true");
            Attr("data-show", "false");

            var header = Add("div");
            header.AddClass("modal-header");
            header.Add("button").Attr("type", "button").AddClass("close").Attr("data-dismiss", "modal").Attr("aria-hidden", "true").Text("x");

            _label = header.Add("h3").Id(labelId);

            _body = Add("div").Id(bodyId).AddClass("modal-body");

            _footer = Add("div").AddClass("modal-footer");
        }

        public HtmlTag Footer
        {
            get { return _footer; }
        }

        public HtmlTag Body
        {
            get { return _body; }
        }

        public HtmlTag Label
        {
            get { return _label; }
        }

        public void AddCloseButton(string text)
        {
            _footer.Add("button").AddClass("btn").Attr("data-dismiss", "modal").Attr("aria-hidden", "true").Text(text);
        }

        public HtmlTag AddFooterButton(string text)
        {
            return Add("button").AddClasses("btn", "btn-primary").Text(text);
        }
    }
}