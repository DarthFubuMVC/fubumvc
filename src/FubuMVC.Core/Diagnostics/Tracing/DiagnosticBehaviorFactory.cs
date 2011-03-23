using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class DiagnosticBehaviorFactory : IBehaviorFactory
    {
        private readonly IBehaviorFactory _inner;
        private readonly IContainerFacility _container;

        public DiagnosticBehaviorFactory(IBehaviorFactory inner, IContainerFacility container)
        {
            _inner = inner;
            _container = container;
        }

        public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            var diagnostics = _container.Get<DiagnosticBehavior>();
            var behavior = _inner.BuildBehavior(arguments, behaviorId);
            diagnostics.Inner = behavior;

            return diagnostics;
        }
    }
}