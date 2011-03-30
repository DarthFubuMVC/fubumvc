using FubuMVC.Core.Urls;
using FubuTestingSupport;

namespace FubuFastPack.Testing
{
    public static class InteractionContextExtensions
    {
        public static IUrlRegistry UseStubUrlRegistry<T>(this InteractionContext<T> context) where T : class
        {
            var registry = new StubUrlRegistry();
            context.Services.Inject<IUrlRegistry>(registry);
            return registry;
        }
    }
}
