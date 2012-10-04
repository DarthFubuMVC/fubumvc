using FubuMVC.Core;
using FubuMVC.Core.Http.Compression;

namespace AspNetApplication
{
    public class AspNetApplicationFubuRegistry : FubuRegistry
    {
        public AspNetApplicationFubuRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();

            Import<ContentCompression>(x => x.Exclude(chain => chain.FirstCall().HandlerType != typeof(CompressedContentController)));
        }
    }
}