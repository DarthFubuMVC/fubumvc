using System.Collections.Generic;
using FubuMVC.Diagnostics.Partials;

namespace FubuMVC.Diagnostics.Notifications
{
	public class NotificationAction<T>
		where T : class, INotificationModel
	{
		private readonly IEnumerable<IPartialDecorator<T>> _decorators;

		public NotificationAction(IEnumerable<IPartialDecorator<T>> decorators)
		{
			_decorators = decorators;
		}

		public T Execute(T input)
		{
			_decorators.Each(decorator => input = decorator.Enrich(input));
			return input;
		}
	}
}