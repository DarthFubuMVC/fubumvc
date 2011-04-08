namespace FubuMVC.Diagnostics.Notifications
{
	public interface INotificationPolicy
	{
		bool Applies();
		INotificationModel Build();
	}
}