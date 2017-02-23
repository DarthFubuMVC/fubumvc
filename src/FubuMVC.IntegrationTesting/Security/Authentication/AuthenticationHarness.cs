using System;
using FubuMVC.Core;
using FubuMVC.Core.Http.Scenarios;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.Security.Authentication.Membership;
using FubuMVC.Core.Urls;
using Xunit;
using StructureMap;

namespace FubuMVC.IntegrationTesting.Security.Authentication
{
    public class AuthenticationHarness : IDisposable
    {
        private IContainer theContainer;
        private FubuRuntime server;

        protected virtual void configure(FubuRegistry registry)
        {
            registry.Actions.IncludeType<SampleController>();


            registry.AlterSettings<AuthenticationSettings>(
                _ => _.Strategies.AddToEnd(MembershipNode.For<InMemoryMembershipRepository>()));
        }

        public BehaviorGraph BehaviorGraph
        {
            get { return Container.GetInstance<BehaviorGraph>(); }
        }

        public AuthenticationHarness()
        {
            var registry = new FubuRegistry();
            configure(registry);

            registry.Features.Authentication.Enable(true);

            server = registry.ToRuntime();
            theContainer = server.Get<IContainer>();

            beforeEach();
        }

        public void Scenario(Action<Scenario> scenario)
        {
            server.Scenario(scenario);
        }

        public void Dispose()
        {
            server.Dispose();
        }

        protected virtual void beforeEach()
        {
        }


        public IContainer Container
        {
            get { return theContainer; }
        }

        public IUrlRegistry Urls
        {
            get { return theContainer.GetInstance<IUrlRegistry>(); }
        }
    }
}