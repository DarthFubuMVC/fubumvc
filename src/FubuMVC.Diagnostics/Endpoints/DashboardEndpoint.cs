using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Notifications;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class DashboardEndpoint
    {
    	private readonly IEnumerable<INotificationPolicy> _policies;

    	public DashboardEndpoint(IEnumerable<INotificationPolicy> policies)
    	{
    		_policies = policies;
    	}

    	[FubuDiagnosticsUrl("~/")]
        public DashboardModel Get(DashboardRequestModel request)
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