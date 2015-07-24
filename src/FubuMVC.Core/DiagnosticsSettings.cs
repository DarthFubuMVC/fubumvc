using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.InMemory;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core
{
    [ApplicationLevel]
    [Title("Diagnostic Tracing and Authorization Configuration")]
    public class DiagnosticsSettings : DescribesItself, IFeatureSettings
    {
        private TraceLevel? _traceLevel;

        public DiagnosticsSettings()
        {
            MaxRequests = 200;

            if (FubuMode.InDevelopment())
            {
                _traceLevel = TraceLevel.Verbose;
            }
        }

        public readonly IList<IAuthorizationPolicy> AuthorizationRights = new List<IAuthorizationPolicy>();

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
            if (FubuMode.InDevelopment() || TraceLevel != TraceLevel.None)
            {
                registry.Policies.ChainSource<DiagnosticChainsSource>();
                registry.Services<TracingServices>();
            }

            if (FubuMode.InDevelopment() || TraceLevel == TraceLevel.Verbose)
            {
                registry.Services(_ =>
                {
                    _.ReplaceService<IBindingLogger, RecordingBindingLogger>();
                    _.ReplaceService<IBindingHistory, BindingHistory>();
                    _.AddService<ILogListener, RequestTraceListener>();
                });
            }
            else if (TraceLevel == TraceLevel.Production)
            {
                registry.Services<ProductionDiagnosticsServices>();
            }
        }
    }

    public class ProductionDiagnosticsServices : ServiceRegistry
    {
        public ProductionDiagnosticsServices()
        {
            AddService<ILogListener, ProductionModeTraceListener>();
        }
    }


    public enum TraceLevel
    {
        Verbose,
        Production,
        None,
    }
}