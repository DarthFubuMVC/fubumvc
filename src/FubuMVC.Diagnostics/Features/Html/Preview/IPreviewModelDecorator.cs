namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public interface IPreviewModelDecorator
    {
        void Enrich(HtmlConventionsPreviewContext context, HtmlConventionsPreviewViewModel model);
    }
}