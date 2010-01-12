using System;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics
{
    public interface IDebugReport
    {
        BehaviorReport StartBehavior(IActionBehavior behavior);
        void EndBehavior();
        void AddDetails(IBehaviorDetails details);
        void MarkException(Exception exception);
        void StartModelBinding(Type type);
        void EndModelBinding(object target);
        void AddBindingDetail(ModelBindingKey binding);
        string Url { get; set; }
        DateTime Time { get; set; }
        IDictionary<string, object> FormData { get; }
        double ExecutionTime { get; }
        void MarkFinished();

        IEnumerable<BehaviorStep> Steps { get; }
    }
}