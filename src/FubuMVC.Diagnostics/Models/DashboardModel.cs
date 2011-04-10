using System.Collections.Generic;
using FubuMVC.Diagnostics.Notifications;

namespace FubuMVC.Diagnostics.Models
{
    public class DashboardModel
    {
		public IEnumerable<INotificationModel> Notifications { get; set; }
    }
}