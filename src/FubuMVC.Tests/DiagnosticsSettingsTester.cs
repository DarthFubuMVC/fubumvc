using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class DiagnosticsSettingsTester
    {
        [Test]
        public void default_request_count_is_200()
        {
            new DiagnosticsSettings()
                .MaxRequests.ShouldBe(200);
        }

        [Test]
        public void add_role()
        {
            var settings = new DiagnosticsSettings();
            settings.RestrictToRole("admin");

            settings.AuthorizationRights.Single().ShouldBeOfType<AllowRole>()
                .Role.ShouldBe("admin");
        }

        [Test]
        public void the_default_trace_level_is_verbose()
        {
            new DiagnosticsSettings()
                .TraceLevel.ShouldBe(TraceLevel.None);
        }

        [Test]
        public void can_override()
        {
            var settings = new DiagnosticsSettings();
            settings.SetIfNone(TraceLevel.Verbose);

            settings.TraceLevel.ShouldBe(TraceLevel.Verbose);

            settings.SetIfNone(TraceLevel.Production);

            settings.TraceLevel.ShouldBe(TraceLevel.Verbose);
        }

        [Test]
        public void level_is_verbose_in_development()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Mode = "development"))
            {
                runtime.Get<DiagnosticsSettings>()
                    .TraceLevel.ShouldBe(TraceLevel.Verbose);
            }
        }
    }
}