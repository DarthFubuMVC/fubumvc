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

namespace FubuMVC.Diagnostics.Tests
{
    [TestFixture]
    public class DiagnosticsSettings_apply_authorization_Tester
    {
        [Test]
        public void authorization_rules_from_settings_are_applied()
        {
            BehaviorGraph authorizedGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.Import<DiagnosticsRegistration>();
                r.AlterSettings<DiagnosticsSettings>(x =>
                {
                    x.RestrictToRule("admin");
                });
            });

            BehaviorGraph notAuthorizedGraph = BehaviorGraph.BuildFrom(r =>
            {
                r.Import<DiagnosticsRegistration>();
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
        BehaviorGraph verboseGraph = BehaviorGraph.BuildFrom(r =>
        {
            r.Import<DiagnosticsRegistration>();
            r.AlterSettings<DiagnosticsSettings>(x =>
            {
                x.TraceLevel = TraceLevel.Verbose;
            });
        });

        BehaviorGraph productionGraph = BehaviorGraph.BuildFrom(r =>
        {
            r.Import<DiagnosticsRegistration>();
            r.AlterSettings<DiagnosticsSettings>(x =>
            {
                x.TraceLevel = TraceLevel.Production;
            });
        });

        BehaviorGraph noneGraph = BehaviorGraph.BuildFrom(r =>
        {
            r.Import<DiagnosticsRegistration>();
            r.AlterSettings<DiagnosticsSettings>(x =>
            {
                x.TraceLevel = TraceLevel.None;
            });
        });

        [Test]
        public void RequestTraceListener_is_added_if_verbose()
        {
            verboseGraph.Services.ServicesFor<ILogListener>()
                .Any(x => x.Type == typeof(RequestTraceListener)).ShouldBeTrue();
        }

        [Test]
        public void RequestTraceListener_should_not_be_registered_if_production()
        {
            productionGraph.Services.ServicesFor<ILogListener>()
                .Any(x => x.Type == typeof(RequestTraceListener)).ShouldBeFalse();
        }

        [Test]
        public void RequestTraceListener_should_not_be_registered_if_none()
        {
            noneGraph.Services.ServicesFor<ILogListener>()
                .Any(x => x.Type == typeof(RequestTraceListener)).ShouldBeFalse();
        }

        [Test]
        public void ProductionModeTraceListener_is_added_if_production()
        {
            productionGraph.Services.ServicesFor<ILogListener>()
                .Any(x => x.Type == typeof(ProductionModeTraceListener)).ShouldBeTrue();
        }

        [Test]
        public void ProductionModeTraceListener_is_not_added_if_verbose()
        {
            verboseGraph.Services.ServicesFor<ILogListener>()
                .Any(x => x.Type == typeof(ProductionModeTraceListener)).ShouldBeFalse();
        }

        [Test]
        public void ProductionModeTraceListener_is_not_added_if_trace_level_is_none()
        {
            noneGraph.Services.ServicesFor<ILogListener>()
                .Any(x => x.Type == typeof(ProductionModeTraceListener)).ShouldBeFalse();
        }


        [Test]
        public void RecordingBinding_logger_is_registered_if_trace_level_is_verbose()
        {
            verboseGraph.Services.DefaultServiceFor<IBindingLogger>()
                .Type.ShouldEqual(typeof (RecordingBindingLogger));

        }

        [Test]
        public void NulloBindingLogger_if_trace_level_is_none()
        {
            noneGraph.Services.DefaultServiceFor<IBindingLogger>()
                .Type.ShouldEqual(typeof (NulloBindingLogger));
        }

        [Test]
        public void NulloBindingLogger_if_trace_level_is_production()
        {
            productionGraph.Services.DefaultServiceFor<IBindingLogger>()
                .Type.ShouldEqual(typeof(NulloBindingLogger));
        }

    }
}