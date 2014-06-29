using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics
{
    public abstract class DiagnosticGroup : IFubuRegistryExtension
    {
        public DiagnosticGroup(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
        public readonly IList<string> Stylesheets = new List<string>();
        public readonly IList<string> Scripts = new List<string>();
        public readonly IList<string> ReactFiles = new List<string>();

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.AlterSettings<DiagnosticsSettings>(x => x.Groups.Add(this));
        }

        public IEnumerable<DiagnosticChain> Chains()
        {
            return _chains;
        }


        private readonly IList<DiagnosticChain> _chains = new List<DiagnosticChain>(); 

        public void Endpoint<T>(string name, Expression<Action<T>> action)
        {
            var actionCall = ActionCall.For(action);
            var chain = new DiagnosticChain(this, actionCall) {RouteName = name};
            _chains.Add(chain);
        }
    }
}