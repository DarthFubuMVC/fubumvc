using HtmlTags;

namespace FubuMVC.Diagnostics.Shared.Tags
{
    public class ControlGroupDivTag : HtmlTag
    {
        private readonly HtmlTag _labelTag;
        private readonly HtmlTag _controlTag;

        public ControlGroupDivTag(string labelText) : this()
        {
            _labelTag.Text(labelText);
        }

        public ControlGroupDivTag(string labelText, string bodyText) : this(labelText)
        {
            ControlTag.Add("div").AddClass("control-label").Text(bodyText);
        }

        public ControlGroupDivTag() : base("div")
        {
            AddClass("control-group");
            _labelTag = Add("label").AddClass("control-label");

            _controlTag = Add("div").AddClass("controls");
        }

        public HtmlTag LabelTag
        {
            get { return _labelTag; }
        }

        public HtmlTag ControlTag
        {
            get { return _controlTag; }
        }
    }
}