using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.InMemory;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsSettings_apply_authorization_Tester
    {
        [Test]
        public void authorization_rules_from_settings_are_applied()
        {
            var authorizedGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.Features.Diagnostics.Configure(x =>
                {
                    x.TraceLevel = TraceLevel.Verbose;
                    x.RestrictToRole("admin");
                });
            });

            var notAuthorizedGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.Features.Diagnostics.Enable(TraceLevel.Verbose);

//                r.AlterSettings<DiagnosticsSettings>(x =>
//                {
//                    x.RestrictToRule("admin");
//                });
            });

            authorizedGraph.Behaviors.OfType<DiagnosticChain>()
                .Each(x => x.Authorization.AllowedRoles().Single().ShouldBe("admin"));

            notAuthorizedGraph.Behaviors.OfType<DiagnosticChain>()
                .Each(x => x.Authorization.HasRules().ShouldBeFalse());
        }
    }

    [TestFixture]
    public class DiagnosticSettings_TraceLevel_Registration_Tester
    {
        private void withTraceLevel(TraceLevel level, Action<IContainer> action)
        {
            FubuMode.Reset();

            var registry = new FubuRegistry();
            registry.Features.Diagnostics.Enable(level);

            using (var runtime = FubuApplication.For(registry).Bootstrap())
            {
                var container = runtime.Factory.Get<IContainer>();
                action(container);
            }
        }

        [Test]
        public void verbose_registrations()
        {
            withTraceLevel(TraceLevel.Verbose, c =>
            {
                c.ShouldHaveRegistration<ILogListener, RequestTraceListener>();
                c.ShouldNotHaveRegistration<ILogListener, ProductionModeTraceListener>();

                c.DefaultRegistrationIs<IBindingLogger, RecordingBindingLogger>();
            });
        }

        [Test]
        public void production_registration()
        {
            withTraceLevel(TraceLevel.Production, c =>
            {
                c.ShouldNotHaveRegistration<ILogListener, RequestTraceListener>();
                c.ShouldHaveRegistration<ILogListener, ProductionModeTraceListener>();

                c.DefaultRegistrationIs<IBindingLogger, NulloBindingLogger>();
            });
        }

        [Test]
        public void trace_level_is_none_registration()
        {
            withTraceLevel(TraceLevel.None, c =>
            {
                c.ShouldNotHaveRegistration<ILogListener, RequestTraceListener>();
                c.ShouldNotHaveRegistration<ILogListener, ProductionModeTraceListener>();

                c.DefaultRegistrationIs<IBindingLogger, NulloBindingLogger>();
            });
        }
    }
}