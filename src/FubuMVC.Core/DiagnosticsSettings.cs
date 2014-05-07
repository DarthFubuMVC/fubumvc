using System;
using System.Collections;
using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Security;

namespace FubuMVC.Core
{
    [ApplicationLevel]
    public class DiagnosticsSettings
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
    }

    public enum TraceLevel
    {
        Verbose,
        Production,
        None,

    }
}