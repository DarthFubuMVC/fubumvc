using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Binding.InMemory;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using Shouldly;
using NUnit.Framework;
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

            authorizedGraph.Chains.OfType<DiagnosticChain>()
                .Each(x => x.Authorization.AllowedRoles().Single().ShouldBe("admin"));

            notAuthorizedGraph.Chains.OfType<DiagnosticChain>()
                .Each(x => x.Authorization.HasRules().ShouldBeFalse());
        }
    }

    [TestFixture]
    public class DiagnosticSettings_TraceLevel_Registration_Tester
    {
        private void withTraceLevel(TraceLevel level, Action<IContainer> action)
        {
            var registry = new FubuRegistry();
            registry.Features.Diagnostics.Enable(level);
            

            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();
                action(container);
            }
        }

        [Test]
        public void default_in_memory_instrumentation_registrations()
        {
            withTraceLevel(TraceLevel.Production, c =>
            {
                c.DefaultSingletonIs<IExecutionLogStorage, InMemoryExecutionLogStorage>();

                c.DefaultSingletonIs<IChainExecutionHistory, ChainExecutionHistory>();
                c.ShouldHaveRegistration<IActivator, PerformanceHistoryQueueActivator>();
                c.DefaultSingletonIs<IPerformanceHistoryQueue, PerformanceHistoryQueue>();

                
            });
        }

        [Test]
        public void verbose_registrations()
        {
            withTraceLevel(TraceLevel.Verbose, c =>
            {
                c.ShouldNotHaveRegistration<ILogListener, ProductionModeTraceListener>();

                c.DefaultRegistrationIs<IBindingLogger, RecordingBindingLogger>();

                c.DefaultSingletonIs<IExecutionLogger, VerboseExecutionLogger>();

                c.ShouldHaveRegistration<ILogListener, ChainExecutionListener>();

                c.DefaultRegistrationIs<IPartialFactory, DiagnosticPartialFactory>();

                c.DefaultRegistrationIs<IChainExecutionLog, NulloChainExecutionLog>();

                c.DefaultRegistrationIs<IEnvelopeLifecycle, EnvelopeLifecycle<VerboseDiagnosticEnvelopeContext>>();
            });
        }

        [Test]
        public void production_registration()
        {
            withTraceLevel(TraceLevel.Production, c =>
            {
                c.ShouldHaveRegistration<ILogListener, ProductionModeTraceListener>();

                c.DefaultRegistrationIs<IBindingLogger, NulloBindingLogger>();

                c.DefaultSingletonIs<IExecutionLogger, ProductionExecutionLogger>();

                c.ShouldNotHaveRegistration<ILogListener, ChainExecutionListener>();

                c.DefaultRegistrationIs<IPartialFactory, PartialFactory>();

                c.DefaultRegistrationIs<IChainExecutionLog, NulloChainExecutionLog>();

                c.DefaultRegistrationIs<IEnvelopeLifecycle, EnvelopeLifecycle<ProductionDiagnosticEnvelopeContext>>();
            });
        }

        [Test]
        public void trace_level_is_none_registration()
        {
            withTraceLevel(TraceLevel.None, c =>
            {
                c.ShouldNotHaveRegistration<ILogListener, ProductionModeTraceListener>();

                c.DefaultRegistrationIs<IBindingLogger, NulloBindingLogger>();

                c.DefaultSingletonIs<IExecutionLogger, NulloExecutionLogger>();

                c.ShouldNotHaveRegistration<ILogListener, ChainExecutionListener>();

                c.DefaultRegistrationIs<IPartialFactory, PartialFactory>();

                c.DefaultRegistrationIs<IChainExecutionLog, NulloChainExecutionLog>();

                c.DefaultRegistrationIs<IEnvelopeLifecycle, EnvelopeLifecycle<EnvelopeContext>>();
            });
        }
    }
}