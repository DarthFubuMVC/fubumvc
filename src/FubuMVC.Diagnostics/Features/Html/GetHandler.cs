namespace FubuMVC.Diagnostics.Features.Html
{
    public class GetHandler
    {
        public HtmlConventionsViewModel Execute(HtmlConventionsRequestModel request)
        {
            return new HtmlConventionsViewModel();
        }
    }
}