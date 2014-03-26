using HtmlTags;

namespace FubuMVC.Core.UI.Forms
{
    public interface IFieldChrome : ITagSource
    {
        HtmlTag LabelTag { get; set; }
        HtmlTag BodyTag { get; set; }
        string Render();
    }
}