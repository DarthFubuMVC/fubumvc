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

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.AlterSettings<DiagnosticsSettings>(x => x.Groups.Add(this));
        }

        public IEnumerable<BehaviorChain> Chains()
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