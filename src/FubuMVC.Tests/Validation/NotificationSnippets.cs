using FubuCore.Reflection;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;

namespace FubuMVC.Tests.Validation
{
	public class NotificationSnippets
	{
		 public static void RegisterMessages()
		 {
			 Accessor accessor = null;
			 var notification = new Notification();

		// SAMPLE: RegisteringNotificationMessages
    // Registering a message
    notification.RegisterMessage(new NotificationMessage(MyTokens.MyKey));

	// Helper to register a message for a given StringToken
	notification.RegisterMessage(MyTokens.MyKey);

	// Register a message for the specific accessor (e.g., "Username is required")
	notification.RegisterMessage(accessor, MyTokens.MyKey);
		// ENDSAMPLE
		 
		 }

		public class MyTokens : StringToken
		{
			public static readonly MyTokens MyKey = new MyTokens("My Key");

			protected MyTokens(string defaultValue)
				: base(null, defaultValue, namespaceByType: true)
			{
			}
		}
	}
}