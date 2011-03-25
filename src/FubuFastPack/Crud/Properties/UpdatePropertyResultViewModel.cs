using FubuValidation;

namespace FubuFastPack.Crud.Properties
{
    public class UpdatePropertyResultViewModel : AjaxContinuation
    {
        public UpdatePropertyResultViewModel()
        {
        }

        public UpdatePropertyResultViewModel(Notification notification, object target, string valueToDisplay)
        {
            WithSubmission(notification, target);
            NewValueToDisplay = valueToDisplay;
        }

        public string NewValueToDisplay { get; set; }
    }
}