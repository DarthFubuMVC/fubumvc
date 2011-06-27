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
			var writer = arguments.Has(typeof(IOutputWriter)) 
                ? arguments.Get<IOutputWriter>() 
                : _container.Get<IOutputWriter>();

            var report = _container.Get<IDebugReport>();
			arguments.Set(typeof(IOutputWriter), new RecordingOutputWriter(report, writer));
            arguments.Set(typeof(IDebugReport), report);

            var behavior = _inner.BuildBehavior(arguments, behaviorId);
            var diagnostics = _container.Get<DiagnosticBehavior>();
            diagnostics.Report = report;
            diagnostics.Inner = behavior;
            return diagnostics;
        }
    }
}