using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Forms;
using HtmlTags.Conventions;

namespace FubuMVC.Core.Validation.Web.UI
{
	public class NotificationSerializationModifier : ITagModifier<FormRequest>
	{
		public bool Matches(FormRequest token)
		{
			return true;
		}

		public void Modify(FormRequest request)
		{
			var validation = request.Chain.ValidationNode();
			if (validation.IsEmpty()) return;

			var notification = request.Services.GetInstance<IFubuRequest>().Get<Notification>();
			if (notification.IsValid()) return;

			var continuation = request.Services.GetInstance<IAjaxContinuationResolver>().Resolve(notification);
			continuation.ValidationOrigin(ValidationOrigin.Server);
			request.CurrentTag.Data("validation-results", continuation.ToDictionary());
		}
	}
}