using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Conneg
{
    public class ConnegBehaviorConvention : IConfigurationAction
    {
        private readonly Func<BehaviorChain, bool> _filter;
        private readonly string _description;

        public ConnegBehaviorConvention(Func<BehaviorChain, bool> filter, string description)
        {
            _filter = filter;
            _description = description;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(_filter).Each(chain => attachMediaHandling(chain, graph.Observer));
        }

        private void attachMediaHandling(BehaviorChain chain, IConfigurationObserver observer)
        {
            var firstAction = chain.FirstCall();
            if (firstAction == null) return;

            observer.RecordCallStatus(firstAction, "Meets criteria {0} for Conneg".ToFormat(_description));

            var node = new ConnegNode(){
                InputType = chain.InputType(),
                OutputType = chain.Calls.Where(x => x.HasOutput).Select(x => x.OutputType()).LastOrDefault()
            };

            firstAction.AddBefore(node);
        }
    }
}