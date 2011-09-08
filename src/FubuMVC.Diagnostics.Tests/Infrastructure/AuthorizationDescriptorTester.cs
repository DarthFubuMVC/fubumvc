using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security;
using FubuMVC.Diagnostics.Core.Infrastructure;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Infrastructure
{
    [TestFixture]
    public class AuthorizationDescriptorTester : InteractionContext<AuthorizationDescriptor>
    {
        [Test]
        public void should_build_configured_endpoint_authorizer()
        {
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(Container));

            var chain = new BehaviorChain();
            MockFor<IServiceLocator>()
                .Expect(locator => locator.GetInstance<IEndPointAuthorizor>(chain.UniqueId.ToString()))
                .Return(MockFor<IEndPointAuthorizor>());

            ClassUnderTest
                .AuthorizorFor(chain)
                .ShouldEqual(MockFor<IEndPointAuthorizor>());
        }
    }
}