using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuMVC.Core.Assets.JavascriptRouting;
using FubuMVC.Core.Diagnostics;
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

        public IList<DiagnosticGroup> Groups = new List<DiagnosticGroup>();

        public DiagnosticJavascriptRoutes ToJavascriptRoutes()
        {
            var routes = new DiagnosticJavascriptRoutes();
            Groups.SelectMany(x => x.Chains()).Each(routes.Add);

            return routes;
        }

        public IEnumerable<string> Stylesheets()
        {
            return Groups.SelectMany(x => x.Stylesheets);
        }

        public IEnumerable<string> Scripts()
        {
            return Groups.SelectMany(x => x.Scripts);
        }

        public IEnumerable<string> ReactFiles()
        {
            return Groups.SelectMany(x => x.ReactFiles);
        }
    }

    public class DiagnosticJavascriptRoutes : JavascriptRouter
    {

    }

    public enum TraceLevel
    {
        Verbose,
        Production,
        None,

    }
}