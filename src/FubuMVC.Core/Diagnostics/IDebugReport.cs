using System;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics
{
    public interface IDebugReport
    {
		Guid Id { get; }
        Guid BehaviorId { get; set; }
        BehaviorReport StartBehavior(IActionBehavior behavior);
        void EndBehavior();
        void AddDetails(IBehaviorDetails details);
        void MarkException(Exception exception);
        void StartModelBinding(Type type);
        void EndModelBinding(object target);
        void AddBindingDetail(IModelBindingDetail binding);
        string Url { get; set; }
        string HttpMethod { get; }
        DateTime Time { get; set; }
        IDictionary<string, object> FormData { get; }
        IDictionary<string, string> Headers { get; }
        double ExecutionTime { get; }
        void MarkFinished();

        IEnumerable<BehaviorStep> Steps { get; }
        void RecordFormData();
    }
}