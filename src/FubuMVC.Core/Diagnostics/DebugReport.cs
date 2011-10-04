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
        public Guid BehaviorId { get; set; }

        public DebugReport()
        {
            Id = Guid.NewGuid();

            FormData = new Dictionary<string, object>();
            Headers = new Dictionary<string, string>();
            Time = DateTime.Now;
        }

        // TODO -- this is a no go for OWIN support
        public void RecordFormData()
        {
            try
            {
                var context = HttpContext.Current;
                if (context != null && context.Request != null)
                {
                    Url = context.Request.Url.PathAndQuery;
                    HttpMethod = context.Request.HttpMethod;
                    populateDictionary(context.Request.Form, (key, value) => FormData.Add(key, value));
                    populateDictionary(context.Request.Headers, (key, value) => Headers.Add(key, value));
                }
            }
            catch (HttpException)
            {
                //Just needs to be here so we can do assert configuration is valid.
            }
        }

        private static void populateDictionary(NameValueCollection collection, Action<string, string> action)
        {
            if (collection == null) return;
            collection
                .AllKeys
                .Where(x => x != null)
                .Each(key => action(key, collection[key]));
        }


        public string Url { get; set; }
        public string HttpMethod { get; private set; }
        public DateTime Time { get; set; }

        public IDictionary<string, object> FormData { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }

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

            AddDetails(new BehaviorStart { BehaviorType = behavior.GetType() });

            return report;
        }

        public void EndBehavior()
        {
            BehaviorReport report = _behaviorStack.Pop();
            report.MarkFinished();
            addDetails(new BehaviorFinish { BehaviorType = report.BehaviorType }, report);
        }

        public void AddDetails(IBehaviorDetails details)
        {
            if (_behaviorStack.Count == 0) return;

            var report = _behaviorStack.Peek();
            addDetails(details, report);
        }

        private void addDetails(IBehaviorDetails details, BehaviorReport report)
        {
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
            _currentModelBinding = null;
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