using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Binding.InMemory;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authorization;
using StructureMap.Configuration.DSL;

namespace FubuMVC.Core
{



    [Title("Diagnostic Tracing and Authorization Configuration")]
    public class DiagnosticsSettings : DescribesItself, IFeatureSettings
    {
        private TraceLevel? _traceLevel;

        public DiagnosticsSettings()
        {
            MaxRequests = 1000;
            InstrumentationServices = new InMemoryInstrumentationServices();
        }

        public readonly IList<IAuthorizationPolicy> AuthorizationRights = new List<IAuthorizationPolicy>();

        public Registry InstrumentationServices { get; set; }

        public void RestrictToRole(string role)
        {
            AuthorizationRights.Add(new AllowRole(role));
        }

        public int MaxRequests { get; set; }

        public TraceLevel TraceLevel
        {
            get { return _traceLevel ?? TraceLevel.None; }
            set { _traceLevel = value; }
        }

        public void SetIfNone(TraceLevel level)
        {
            if (!_traceLevel.HasValue)
            {
                _traceLevel = level;
            }
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "Governs the behavior and verbosity of the runtime diagnostics";
            description.Properties["Tracing Level"] = TraceLevel.ToString();
            description.Properties["Maximum Number of Requests to Keep"] = MaxRequests.ToString();
            description.AddList("Authorization Rules for Diagnostics", AuthorizationRights);
        }

        void IFeatureSettings.Apply(FubuRegistry registry)
        {
            if (registry.Mode.InDevelopment() || TraceLevel != TraceLevel.None)
            {
                registry.Policies.ChainSource<DiagnosticChainsSource>();
                registry.Services.ReplaceService<IBindingHistory, BindingHistory>();

                registry.Services.IncludeRegistry(InstrumentationServices);

                

            }

            if (registry.Mode.InDevelopment() || TraceLevel == TraceLevel.Verbose)
            {
                registry.Services.ReplaceService<IBindingLogger, RecordingBindingLogger>();
                registry.Services.ReplaceService<IBindingHistory, BindingHistory>();
                registry.Services.ForSingletonOf<IExecutionLogger>().Use<VerboseExecutionLogger>();
                registry.Services.AddService<ILogListener, ChainExecutionListener>();

                registry.Services.ReplaceService<IPartialFactory, DiagnosticPartialFactory>();
            }
            else if (TraceLevel == TraceLevel.Production)
            {
                registry.Services.IncludeRegistry<ProductionDiagnosticsServices>();
            }
            else
            {
                registry.Services.ForSingletonOf<IExecutionLogger>().Use<NulloExecutionLogger>();
            }
        }

        public Func<IDictionary<string, object>, Task> WrapAppFunc(FubuRuntime runtime, Func<IDictionary<string, object>, Task> inner)
        {
            if (TraceLevel == TraceLevel.None) return inner;

            var logger = runtime.Get<IExecutionLogger>();

            return env =>
            {
                var log = new ChainExecutionLog();
                env.Log(log);

                return inner(env).ContinueWith(t =>
                {
                    logger.Record(log, env);
                });
            };
        }
    }

    public class ProductionDiagnosticsServices : ServiceRegistry
    {
        public ProductionDiagnosticsServices()
        {
            AddService<ILogListener, ProductionModeTraceListener>();
            ForSingletonOf<IExecutionLogger>().Use<ProductionExecutionLogger>();
        }
    }


    public enum TraceLevel
    {
        Verbose,
        Production,
        None,
    }
}