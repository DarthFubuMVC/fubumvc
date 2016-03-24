using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Validation.Web.Remote
{
    // TODO -- get rid of this
    public class RemoteRuleGraphActivator : IActivator
    {
        private readonly ValidationGraph _graph;
        private readonly RemoteRuleGraph _remoteGraph;
        private readonly BehaviorGraph _behaviorGraph;
        private readonly IRemoteRuleQuery _remotes;
        private readonly ITypeDescriptorCache _properties;

        public RemoteRuleGraphActivator(ValidationGraph graph, RemoteRuleGraph remoteGraph, BehaviorGraph behaviorGraph, IRemoteRuleQuery remotes, ITypeDescriptorCache properties)
        {
            _graph = graph;
            _remoteGraph = remoteGraph;
            _behaviorGraph = behaviorGraph;
            _remotes = remotes;
            _properties = properties;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            // Find the input models that have remote rules
            // "Bake" them into the remote graph
            _behaviorGraph
                .Actions()
                .Each(FillRules);
        }

        public void FillRules(ActionCall call)
        {
            var input = call.InputType();
            if (input == null) return;

            _properties.ForEachProperty(input, property =>
            {
                var accessor = new SingleProperty(property);
                var rules = _graph
                    .Query()
                    .RulesFor(input, accessor)
                    .Where(rule => _remotes.IsRemote(rule));

                rules.Each(rule => _remoteGraph.RegisterRule(accessor, rule));
            });
        }
    }
}