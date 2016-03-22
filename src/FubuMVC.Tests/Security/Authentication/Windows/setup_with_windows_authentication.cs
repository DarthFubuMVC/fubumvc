using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authentication.Windows;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Security.Authentication.Windows
{
    [TestFixture]
    public class setup_with_windows_authentication
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            var registry = new FubuRegistry();
            registry.Import<ApplyWindowsAuthentication>();

            theGraph = BehaviorGraph.BuildFrom(registry);
        }

        [Test]
        public void the_windows_action_call_is_registered()
        {
            theGraph.ChainFor<WindowsController>(x => x.Login(null)).ShouldNotBeNull();
        }

    }
}