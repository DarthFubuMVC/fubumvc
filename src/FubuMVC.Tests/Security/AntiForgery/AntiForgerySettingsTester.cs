using FubuMVC.Core.Security.AntiForgery;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.AntiForgery
{
    public class AntiForgerySettingsTester
    {
        [Test]
        public void anti_forgery_is_disabled_by_default()
        {
            new AntiForgerySettings().Enabled.ShouldBeFalse();
        } 
    }
}