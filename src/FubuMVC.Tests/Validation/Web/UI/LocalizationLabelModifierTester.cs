using FubuMVC.Core.Localization;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Web.UI;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    
    public class LocalizationLabelModifierTester : ValidationElementModifierContext<LocalizationLabelModifier>
    {
        [Fact]
        public void adds_the_localized_label_data_attribute()
        {
            var theRequest = ElementRequest.For(new TargetModel(), x => x.FirstName);
            var label = LocalizationManager.GetHeader(theRequest.Accessor.InnerProperty);
            tagFor(theRequest).Data(LocalizationLabelModifier.LocalizedLabelKey).ShouldBe(label);
        }

        public class TargetModel
        {
            public string FirstName { get; set; }
        }
    }
}