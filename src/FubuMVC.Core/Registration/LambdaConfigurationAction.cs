using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration
{
    [Title("Explicit configuration action")]
    public class LambdaConfigurationAction : IConfigurationAction
    {
        private readonly Action<BehaviorGraph> _action;

        public LambdaConfigurationAction(Action<BehaviorGraph> action)
        {
            _action = action;
        }

        public void Configure(BehaviorGraph graph)
        {
            _action(graph);
        }
    }

}