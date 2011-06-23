using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Configuration.Policies;

namespace FubuMVC.Diagnostics.Navigation
{
    public abstract class NavigationItemBase : INavigationItemAction
    {
        private readonly BehaviorGraph _graph;
        private readonly IEndpointService _endpointService;
        private readonly Lazy<ActionCall> _actionCall;

        protected NavigationItemBase(BehaviorGraph graph, IEndpointService endpointService)
        {
            _graph = graph;
            _endpointService = endpointService;
            _actionCall = new Lazy<ActionCall>(() => _graph.Behaviors.SingleOrDefault(chain => chain.InputType() == inputModel().GetType()).FirstCall());
        }

        protected abstract object inputModel();

        public virtual string Text()
        {
            return _actionCall
                .Value
                .HandlerType
                .Name
                .Replace(DiagnosticsEndpointUrlPolicy.ENDPOINT, "");
        }

        public virtual string Url()
        {
            return _endpointService
                .EndpointFor(inputModel())
                .Url;
        }
    }
}