using System;

namespace FubuMVC.Core.Registration
{
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