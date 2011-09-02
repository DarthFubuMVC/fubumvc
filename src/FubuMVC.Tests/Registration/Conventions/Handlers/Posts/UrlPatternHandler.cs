using FubuMVC.Core;

namespace FubuMVC.Tests.Registration.Conventions.Handlers.Posts
{
    public class UrlPatternHandler
    {
        [UrlPattern("some-crazy-url/as-a-subfolder")]
        public JsonResponse Execute()
        {
            return new JsonResponse();
        }
    }
}