using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class DiagnosticsSettingsTester
    {
        [SetUp]
        public void SetUp()
        {
            FubuMode.Reset();
        }

        [Test]
        public void default_request_count_is_200()
        {
            new DiagnosticsSettings()
                .MaxRequests.ShouldEqual(200);
        }

        [Test]
        public void add_role()
        {
            var settings = new DiagnosticsSettings();
            settings.RestrictToRule("admin");

            settings.AuthorizationRights.Single().ShouldBeOfType<AllowRole>()
                .Role.ShouldEqual("admin");
        }

        [Test]
        public void the_default_trace_level_is_verbose()
        {
            new DiagnosticsSettings()
                .TraceLevel.ShouldEqual(TraceLevel.None);
        }

        [Test]
        public void can_override()
        {
            var settings = new DiagnosticsSettings();
            settings.SetIfNone(TraceLevel.Verbose);

            settings.TraceLevel.ShouldEqual(TraceLevel.Verbose);

            settings.SetIfNone(TraceLevel.Production);

            settings.TraceLevel.ShouldEqual(TraceLevel.Verbose);
        }

        [Test]
        public void level_is_verbose_in_development()
        {
            FubuMode.SetUpForDevelopmentMode();

            new DiagnosticsSettings().TraceLevel.ShouldEqual(TraceLevel.Verbose);
        }
    }
}