using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class Activity
    {
        private readonly double _start;
        
        private double _end;
        private readonly ISubject _subject;

        public Activity(ISubject subject, double start)
        {
            _start = start;
            _subject = subject;
        }


        public IList<Activity> Nested = new List<Activity>(); 


        public ISubject Subject
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

        public double InnerTime
        {
            get { return Duration - Nested.Sum(x => x.Duration); }
        }

        public IEnumerable<Activity> AllActivities()
        {
            yield return this;

            foreach (var activity in Nested)
            {
                yield return activity;

                foreach (var child in children())
                {
                    yield return child;
                }
            }
        }

        private IEnumerable<Activity> children()
        {
            foreach (var child in Nested)
            {
                yield return child;

                foreach ( var descendent in child.children())
                {
                    yield return descendent;
                }
            }
        } 
    }
}