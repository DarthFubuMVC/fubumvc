using FubuMVC.Core;
using FubuMVC.Core.Http.Compression;

namespace AspNetApplication
{
    public class AspNetApplicationFubuRegistry : FubuRegistry
    {
        public AspNetApplicationFubuRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();
            Actions.IncludeClassesSuffixedWithEndpoint();

            Policies.Local.Add(x => {
                x.Where.AnyActionMatches(call => call.HandlerType == typeof (CompressedContentController));
                x.ContentCompression.Apply();
            });
        }
    }
}