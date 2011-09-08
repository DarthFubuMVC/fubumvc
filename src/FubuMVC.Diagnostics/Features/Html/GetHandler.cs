namespace FubuMVC.Diagnostics.Features.Html
{
    public class GetHandler
    {
        public HtmlConventionsViewModel Get(HtmlConventionsRequestModel request)
        {
            return new HtmlConventionsViewModel();
        }
    }
}