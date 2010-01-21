using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.UI.Forms
{
    public interface ILabelAndFieldLayout : ITagSource
    {
        HtmlTag LabelTag { get; set; }
        HtmlTag BodyTag { get; set; }
        void WrapBody(HtmlTag tag);
        HtmlTag WrapBody(string tagName);
        void SetLabelText(string text);
    }
}