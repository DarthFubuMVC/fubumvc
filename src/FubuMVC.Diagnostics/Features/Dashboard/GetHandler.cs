using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Notifications;

namespace FubuMVC.Diagnostics.Features.Dashboard
{
    public class GetHandler
    {
        private readonly IEnumerable<INotificationPolicy> _policies;

        public GetHandler(IEnumerable<INotificationPolicy> policies)
        {
            _policies = policies;
        }

        [FubuDiagnosticsUrl("~/")]
        public DashboardModel Execute(DashboardRequestModel request)
        {
            return new DashboardModel
                    {
                        Notifications = _policies
                                            .Where(policy => policy.Applies())
                                            .Select(p => p.Build())
                    };
        }
    }
}