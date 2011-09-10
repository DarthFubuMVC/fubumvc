namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public interface IHtmlConventionsPreviewContextFactory
    {
        HtmlConventionsPreviewContext BuildFromPath(string modelPath);
    }
}