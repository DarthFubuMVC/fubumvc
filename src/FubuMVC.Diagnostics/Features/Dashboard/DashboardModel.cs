using System.Collections.Generic;
using FubuMVC.Diagnostics.Notifications;

namespace FubuMVC.Diagnostics.Features.Dashboard
{
    public class DashboardModel
    {
		public IEnumerable<INotificationModel> Notifications { get; set; }
    }
}