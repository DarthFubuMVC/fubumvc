using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public interface IPartialFactory
    {
        IActionBehavior BuildPartial(Type inputType);
        IActionBehavior BuildPartial(ActionCall call);
    }

    public class PartialFactory : IPartialFactory
    {
        private readonly ServiceArguments _arguments;
        private readonly IBehaviorFactory _factory;
        private readonly BehaviorGraph _graph;

        public PartialFactory(BehaviorGraph graph, IBehaviorFactory factory, ServiceArguments arguments)
        {
            _graph = graph;
            _factory = factory;
            _arguments = arguments;
        }

        public IActionBehavior BuildPartial(Type inputType)
        {
            Guid id = _graph.IdForType(inputType);
            return _factory.BuildBehavior(_arguments, id);
        }

        public IActionBehavior BuildPartial(ActionCall call)
        {
            Guid id = _graph.IdForCall(call);
            return _factory.BuildBehavior(_arguments, id);
        }
    }
}