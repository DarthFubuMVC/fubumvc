using FubuMVC.Core.Continuations;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    public class PostHandler
    {
        public FubuContinuation Execute(HtmlConventionsPreviewInputModel input)
        {
            return FubuContinuation.RedirectTo(new HtmlConventionsPreviewRequestModel { OutputModel = input.OutputModel });
        } 
    }
}