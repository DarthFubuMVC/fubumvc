using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Diagnostics
{
    public class BehaviorStep
    {
        public BehaviorReport Behavior { get; set; }
        public IBehaviorDetails Details { get; set; }
    }

    public class DebugReport : TimedReport, IEnumerable<BehaviorReport>, IDebugReport
    {
        private readonly IList<BehaviorReport> _behaviors = new List<BehaviorReport>();
        private readonly Stack<BehaviorReport> _behaviorStack = new Stack<BehaviorReport>();
        private readonly IList<BehaviorStep> _steps = new List<BehaviorStep>();
        private ModelBindingReport _currentModelBinding;

        public Guid Id { get; private set; }

        public DebugReport()
        {
            Id = Guid.NewGuid();

            FormData = new Dictionary<string, object>();
            Time = DateTime.Now;

            try
            {
                var context = HttpContext.Current;
                if (context != null)
                {
                    Url = context.Request.Url.PathAndQuery;
                    NameValueCollection formData = context.Request.Form;
                    formData.AllKeys.Where(x => x != null).Each(key =>
                    {
                        FormData.Add(key, formData[key]);
                    });
                }
            }
            catch (HttpException)
            {
                //Just needs to be here so we can do assert configuration is valid.
            }
        }


        public string Url { get; set; }
        public DateTime Time { get; set; }

        public IDictionary<string, object> FormData { get; private set; }

        public IEnumerable<BehaviorStep> Steps
        {
            get { return _steps; } }

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public BehaviorReport StartBehavior(IActionBehavior behavior)
        {
            var report = new BehaviorReport(behavior);
            _behaviors.Add(report);
            _behaviorStack.Push(report);

            AddDetails(new BehaviorStart());

            return report;
        }

        public void EndBehavior()
        {
            BehaviorReport report = _behaviorStack.Pop();
            report.MarkFinished();
        }

        public void AddDetails(IBehaviorDetails details)
        {
            if (_behaviorStack.Count == 0) return;

            BehaviorReport report = _behaviorStack.Peek();

            _steps.Add(new BehaviorStep
            {
                Behavior = report,
                Details = details
            });

            report.AddDetail(details);
        }

        public void MarkException(Exception exception)
        {
            var details = new ExceptionReport
            {
                Text = exception.ToString()
            };

            AddDetails(details);
        }

        public void StartModelBinding(Type type)
        {
            _currentModelBinding = new ModelBindingReport()
            {
                BoundType = type
            };
            AddDetails(_currentModelBinding);
        }

        public void EndModelBinding(object target)
        {
            if (_currentModelBinding == null) return;

            _currentModelBinding.StoredObject = target;
            _currentModelBinding.MarkFinished();
        }

        public void AddBindingDetail(IModelBindingDetail binding)
        {
            if (_currentModelBinding != null) _currentModelBinding.Add(binding);
        }

        public IEnumerator<BehaviorReport> GetEnumerator()
        {
            return _behaviors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}