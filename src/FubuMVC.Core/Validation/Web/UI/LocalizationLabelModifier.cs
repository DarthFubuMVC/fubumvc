using FubuMVC.Core.Localization;
using FubuMVC.Core.UI.Elements;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class LocalizationLabelModifier : InputElementModifier
    {
        public const string LocalizedLabelKey = "localized-label";

        protected override void modify(ElementRequest request)
        {
            var label = LocalizationManager.GetHeader(request.Accessor.InnerProperty);
            request.CurrentTag.Data(LocalizedLabelKey, label);
        }
    }
}