using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class Activity
    {
        private readonly double _start;
        
        private double _end;
        private readonly IList<RequestStep> _steps = new List<RequestStep>();
        private readonly object _subject;

        public Activity(object subject, double start)
        {
            _start = start;
            _subject = subject;
        }

        public IList<Activity> Nested = new List<Activity>(); 

        public void AppendLog(double time, object log)
        {
            var step = new RequestStep(time, log) {Activity = this};
            _steps.Add(step);
        }



        public IEnumerable<RequestStep> AllSteps()
        {
            foreach (var step in _steps)
            {
                yield return step;
            }

            foreach (var activity in Nested)
            {
                foreach (var step in activity.AllSteps())
                {
                    yield return step;
                }
            }
        } 

        public IEnumerable<RequestStep> Steps
        {
            get { return _steps; }
        }

        public object Subject
        {
            get { return _subject; }
        }

        public void MarkEnd(double end)
        {
            _end = end;
        }

        public double Start
        {
            get { return _start; }
        }

        public double End
        {
            get { return _end; }
        }

        public double Duration
        {
            get { return _end - _start; }
        }
    }
}