using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication.Windows;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    
    public class setup_with_windows_authentication
    {
        private BehaviorGraph theGraph;

        public setup_with_windows_authentication()
        {
            var registry = new FubuRegistry();
            registry.Import<ApplyWindowsAuthentication>();

            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        [Fact]
        public void the_windows_action_call_is_registered()
        {
            theGraph.ChainFor<WindowsController>(x => x.Login(null)).ShouldNotBeNull();
        }

    }
}