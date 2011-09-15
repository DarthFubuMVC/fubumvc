using System;
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
            var thereAreAnyNonDiagnosticsChains = nonDiagnosticsChains().Any();
            return !thereAreAnyNonDiagnosticsChains;
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
            // No action calls. Check for partials, too
            var output = chain.InputType();
            if (output == null)
            {
                // try for a handler
                var call = chain.FirstCall();
                if(call == null)
                {
                    // sanity check?
                    return false;
                }

                return checkAssembly(call.HandlerType);
            }

            return checkAssembly(output);
        }

        private bool checkAssembly(Type type)
        {
            return type.Assembly.Equals(GetType().Assembly) || type.Assembly.Equals(typeof(INotificationPolicy).Assembly)
                    || type.Assembly.Equals(typeof(ActionCall).Assembly);
        }
    }
}