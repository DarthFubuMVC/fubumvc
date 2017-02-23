using FubuMVC.Core.Security.AntiForgery;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.AntiForgery
{
    public class AntiForgerySettingsTester
    {
        [Fact]
        public void anti_forgery_is_disabled_by_default()
        {
            new AntiForgerySettings().Enabled.ShouldBeFalse();
        } 
    }
}