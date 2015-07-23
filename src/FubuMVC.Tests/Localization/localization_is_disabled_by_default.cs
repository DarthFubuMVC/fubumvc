using FubuMVC.Core.Localization;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Localization
{
    [TestFixture]
    public class localization_is_disabled_by_default
    {
        [Test]
        public void default_value_is_disabled()
        {
            new LocalizationSettings().Enabled.ShouldBeFalse();
        }
    }
}