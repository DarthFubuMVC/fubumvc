using FubuMVC.Diagnostics.Features.Dashboard;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Notifications;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Dashboard
{
    [TestFixture]
    public class DashboardHandlerTester : InteractionContext<GetHandler>
    {
        [Test]
        public void should_not_build_notifications_that_apply()
        {
            MockFor<INotificationPolicy>()
                .Expect(p => p.Applies())
                .Return(false);

            ClassUnderTest
                .Execute(new DashboardRequestModel())
                .Notifications
                .ShouldHaveCount(0);
        }

        [Test]
        public void should_build_notifications_that_apply()
        {
            MockFor<INotificationPolicy>()
                .Expect(p => p.Applies())
                .Return(true);

            MockFor<INotificationPolicy>()
                .Expect(p => p.Build())
                .Return(MockFor<INotificationModel>());

            ClassUnderTest
                .Execute(new DashboardRequestModel())
                .Notifications
                .ShouldContain(MockFor<INotificationModel>());
        }
    }
}