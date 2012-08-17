using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Etags;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class Caching_services_are_registered
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            BehaviorGraph.BuildEmptyGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void headers_cache_is_registered_as_a_singleton()
        {
            registeredTypeIs<IHeadersCache, HeadersCache>();

            ServiceRegistry.ShouldBeSingleton(typeof(HeadersCache)).ShouldBeTrue();
        }
    }
}