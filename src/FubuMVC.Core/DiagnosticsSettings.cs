using System.Collections.Generic;
using System.Diagnostics;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security;

namespace FubuMVC.Core
{
    [ApplicationLevel]
    [Title("Diagnostic Tracing and Authorization Configuration")]
    public class DiagnosticsSettings : DescribesItself
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

        public void RestrictToRule(string role)
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
    }


    public enum TraceLevel
    {
        Verbose,
        Production,
        None,
    }
}