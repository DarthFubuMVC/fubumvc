namespace FubuMVC.Diagnostics.Notifications
{
	public class NoOutputsNotification : INotificationModel
	{
		public int BehaviorCount { get; set; }
		public FilterLink Filter { get; set; }
	}
}