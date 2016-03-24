using FubuMVC.Core.Localization;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.Validation.Web.UI;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web.UI
{
    [TestFixture]
    public class LocalizationLabelModifierTester : ValidationElementModifierContext<LocalizationLabelModifier>
    {
        [Test]
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