using HtmlTags;

namespace FubuMVC.Core.UI.Forms
{
    public interface ILabelAndFieldLayout : ITagSource
    {
        HtmlTag LabelTag { get; set; }
        HtmlTag BodyTag { get; set; }
        string Render();
    }
}