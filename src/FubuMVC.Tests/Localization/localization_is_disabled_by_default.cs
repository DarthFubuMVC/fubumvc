using FubuMVC.Core.Localization;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Localization
{
    
    public class localization_is_disabled_by_default
    {
        [Fact]
        public void default_value_is_disabled()
        {
            new LocalizationSettings().Enabled.ShouldBeFalse();
        }
    }
}