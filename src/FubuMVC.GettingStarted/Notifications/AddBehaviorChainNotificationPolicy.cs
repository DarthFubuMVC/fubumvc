using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Core.Configuration;
using FubuMVC.Diagnostics.Notifications;

namespace FubuMVC.GettingStarted.Notifications
{
    public class AddBehaviorChainNotificationPolicy : INotificationPolicy
    {
        private readonly BehaviorGraph _graph;

        public AddBehaviorChainNotificationPolicy(BehaviorGraph graph)
        {
            _graph = graph;
        }

        public bool Applies()
        {
            return !nonDiagnosticsChains().Any();
        }

        public INotificationModel Build()
        {
            return new AddBehaviorChainNotification();
        }

        private IEnumerable<BehaviorChain> nonDiagnosticsChains()
        {
            return _graph
                .Behaviors
                .Where(chain => !isDiagnosticsChain(chain));
        }

        private bool isDiagnosticsChain(BehaviorChain chain)
        {
            var assembly = GetType().Assembly;
            var call = chain.FirstCall();

            // First we'll try to grab the action call
            if (call == null)
            {
                // No action calls. Check for partials, too
                var output = chain.InputType();
                if (output == null)
                {
                    // No action calls and no partials? You need some suggeestions
                    return false;
                }

                return output.Assembly.Equals(assembly) || output.Assembly.Equals(typeof(ActionCall).Assembly);
            }

            // Make sure the ActionCall is coming from a custom assembly
            return call.IsDiagnosticsCall() || call.IsInternalFubuAction() || call.HandlerType.Assembly.Equals(assembly);
        }
    }
}