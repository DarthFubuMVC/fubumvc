using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Binding.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Diagnostics
{
    public class BehaviorStep
    {
        public BehaviorReport Behavior { get; set; }
        public IBehaviorDetails Details { get; set; }
    }

    public class BindingHistory : IBindingHistory
    {
        private readonly DebugReport _report;

        public BindingHistory(DebugReport report)
        {
            _report = report;
        }

        public void Store(BindingReport report)
        {
            // do nothing
        }
    }

    public class DebugReport : TimedReport, IEnumerable<BehaviorReport>, IDebugReport
    {
        private readonly Stack<BehaviorReport> _behaviorStack = new Stack<BehaviorReport>();
        private readonly IList<BehaviorReport> _behaviors = new List<BehaviorReport>();
        private readonly IRequestData _data;
        private readonly ICurrentHttpRequest _request;
        private readonly IList<BehaviorStep> _steps = new List<BehaviorStep>();
        private ModelBindingReport _currentModelBinding;

        public DebugReport(IRequestData data, ICurrentHttpRequest request)
        {
            _data = data;
            _request = request;
            Id = Guid.NewGuid();

            FormData = new Dictionary<string, object>();
            Headers = new Dictionary<string, string>();
            Time = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public Guid BehaviorId { get; set; }


        public void RecordFormData()
        {
            try
            {
                //throw new NotImplementedException();

                //// TODO -- Be nice to have better stuff in FubuCore
                //var requestData = _dictionary.DataFor(RequestDataSource.Request.ToString());
                //requestData.GetKeys().ToList().Each(key => FormData.Add(key, requestData.Value(key)));

                //var requestData2 = _dictionary.DataFor(RequestDataSource.Header.ToString());
                //requestData2.GetKeys().ToList().Each(key => Headers.Add(key, (requestData2.Value(key) ?? string.Empty).ToString()));

                Url = _request.RawUrl();
                HttpMethod = _request.HttpMethod();
            }
            catch (HttpException)
            {
                //Just needs to be here so we can do assert configuration is valid.
            }
        }

        public string Url { get; set; }
        public string HttpMethod { get; private set; }
        public DateTime Time { get; set; }

        public IDictionary<string, object> FormData { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }

        public IEnumerable<BehaviorStep> Steps
        {
            get { return _steps; }
        }

        public BehaviorReport StartBehavior(IActionBehavior behavior)
        {
            var report = new BehaviorReport(behavior);
            _behaviors.Add(report);
            _behaviorStack.Push(report);

            AddDetails(new BehaviorStart{
                BehaviorType = behavior.GetType()
            });

            return report;
        }

        public void EndBehavior()
        {
            var report = _behaviorStack.Pop();
            report.MarkFinished();
            addDetails(new BehaviorFinish{
                BehaviorType = report.BehaviorType
            }, report);
        }

        public void AddDetails(IBehaviorDetails details)
        {
            if (_behaviorStack.Count == 0) return;

            var report = _behaviorStack.Peek();
            addDetails(details, report);
        }

        public void MarkException(Exception exception)
        {
            var details = new ExceptionReport{
                Text = exception.ToString()
            };

            AddDetails(details);
        }

        public void StartModelBinding(Type type)
        {
            _currentModelBinding = new ModelBindingReport{
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

        public void AcceptVisitor(IBehaviorDetailsVisitor visitor)
        {
            throw new NotImplementedException();
        }

        private void addDetails(IBehaviorDetails details, BehaviorReport report)
        {
            _steps.Add(new BehaviorStep{
                Behavior = report,
                Details = details
            });

            report.AddDetail(details);
        }
    }
}