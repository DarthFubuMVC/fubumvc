using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Infrastructure
{
    [TestFixture]
    public class AuthorizationDescriptorTester : InteractionContext<AuthorizationDescriptor>
    {
        [Test]
        public void should_build_configured_endpoint_authorizer()
        {
            Assert.Fail("NWO");
            //ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(Container));

            //var chain = new BehaviorChain();
            //MockFor<IServiceLocator>()
            //    .Expect(locator => locator.GetInstance<IEndPointAuthorizor>(chain.UniqueId.ToString()))
            //    .Return(MockFor<IEndPointAuthorizor>());

            //ClassUnderTest
            //    .AuthorizorFor(chain)
            //    .ShouldEqual(MockFor<IEndPointAuthorizor>());
        }
    }
}