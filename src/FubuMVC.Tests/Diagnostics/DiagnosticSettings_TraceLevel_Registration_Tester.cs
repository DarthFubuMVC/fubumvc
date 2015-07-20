using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Binding.InMemory;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using TraceLevel = FubuMVC.Core.TraceLevel;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsSettings_apply_authorization_Tester
    {
        [Test]
        public void authorization_rules_from_settings_are_applied()
        {
            BehaviorGraph authorizedGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.AlterSettings<DiagnosticsSettings>(x =>
                {
                    x.TraceLevel = TraceLevel.Verbose;
                    x.RestrictToRole("admin");
                });
            });

            BehaviorGraph notAuthorizedGraph = BehaviorGraph.BuildFrom(r =>
            {
                  r.AlterSettings<DiagnosticsSettings>(x =>
                  {
                      x.TraceLevel = TraceLevel.Verbose;
                  });
//                r.AlterSettings<DiagnosticsSettings>(x =>
//                {
//                    x.RestrictToRule("admin");
//                });
            });

            authorizedGraph.Behaviors.OfType<DiagnosticChain>()
                .Each(x => x.Authorization.AllowedRoles().Single().ShouldEqual("admin"));

            notAuthorizedGraph.Behaviors.OfType<DiagnosticChain>()
                .Each(x => x.Authorization.HasRules().ShouldBeFalse());
        }
    }

    [TestFixture]
    public class DiagnosticSettings_TraceLevel_Registration_Tester
    {
        private BehaviorGraph verboseGraph;
        private BehaviorGraph productionGraph;
        private BehaviorGraph noneGraph;

        [TestFixtureSetUp]
        public void SetUp()
        {
            FubuMode.Reset();
            verboseGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.AlterSettings<DiagnosticsSettings>(x =>
                {
                    x.TraceLevel = TraceLevel.Verbose;
                });
            });

            productionGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.AlterSettings<DiagnosticsSettings>(x =>
                {
                    x.TraceLevel = TraceLevel.Production;
                });
            });

            noneGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.AlterSettings<DiagnosticsSettings>(x =>
                {
                    x.TraceLevel = TraceLevel.None;
                });
            });
        }

        private void withTraceLevel(TraceLevel level, Action<IContainer> action)
        {
            var registry = new FubuRegistry();
            registry.AlterSettings<DiagnosticsSettings>(_ => _.TraceLevel = level);

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