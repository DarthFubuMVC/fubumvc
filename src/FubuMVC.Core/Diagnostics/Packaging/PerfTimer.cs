using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Util;
using FubuCore.Util.TextWriting;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    public interface IPerfTimer
    {
        void Record(string text, Action action);
        T Record<T>(string text, Func<T> func);
    }

    public class PerfTimer : IPerfTimer
    {
        public static readonly string Started = "Started";
        public static readonly string Finished = "Finished";
        public static readonly string Marked = "Marked";

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private ConcurrentQueue<Checkpoint> _checkpoints = new ConcurrentQueue<Checkpoint>();
        private string _description;

        public void Start(string description)
        {
            _description = description;
            

            _stopwatch.Reset();
            _checkpoints = new ConcurrentQueue<Checkpoint>();

            _checkpoints.Enqueue(new Checkpoint(Started, _description, 0));

            _stopwatch.Start();
            
        }

        public void Stop()
        {
            
            _stopwatch.Stop();
            add(Finished, _description);
        }

        public long TimeEllapsedInMilliseconds()
        {
            return _stopwatch.ElapsedMilliseconds;
        }

        private void add(string status, string text)
        {
            var checkpoint = new Checkpoint(status, text, _stopwatch.ElapsedMilliseconds);
            _checkpoints.Enqueue(checkpoint);
        }

        public void Mark(string text)
        {
            add(Marked, text);
        }

        public void Record(string text, Action action)
        {
            add(Started, text);
            action();
            add(Finished, text);
        }

        public T Record<T>(string text, Func<T> func)
        {
            add(Started, text);
            try
            {
                return func();
            }
            finally
            {
                add(Finished, text);
            }
        }

        public IEnumerable<TimedStep> TimedSteps()
        {
            var steps = new Cache<string, TimedStep>(text => new TimedStep {Text = text});
            _checkpoints.Where(x => x.Status == Started).Each(x => { steps[x.Text].Start = x.Time; });

            _checkpoints.Where(x => x.Status == Finished).Each(x => { steps[x.Text].Finished = x.Time; });

            _checkpoints.Where(x => x.Status == Marked).Each(x => {
                var step = steps[x.Text];
                step.Start = step.Finished = x.Time;
            });

            return steps;
        }

        public TextReport DisplayTimings<T>(Func<TimedStep, T> sort)
        {
            var ordered = TimedSteps().OrderBy(sort).ToArray();

            return displayTimings(ordered);
        }

        private static TextReport displayTimings(IEnumerable<TimedStep> ordered)
        {
            var writer = new FubuCore.Util.TextWriting.TextReport();
            writer.StartColumns(new Column(ColumnJustification.left, 0, 3), new Column(ColumnJustification.right, 0, 3),
                new Column(ColumnJustification.right, 0, 3), new Column(ColumnJustification.right, 0, 3));
            writer.AddColumnData("Description", "Start", "Finish", "Duration");
            writer.AddDivider('-');

            ordered.Each(
                x => { writer.AddColumnData(x.Text, x.Start.ToString(), x.Finished.ToString(), x.Duration().ToString()); });

            return writer;
        }

        public TextReport DisplayTimings()
        {
            var ordered = TimedSteps().ToList();
            ordered.Sort();

            return displayTimings(ordered);
        }

        public class Checkpoint
        {
            private readonly string _status;
            private readonly string _text;
            private readonly long _time;

            public Checkpoint(string status, string text, long time)
            {
                _status = status;
                _text = text;
                _time = time;
            }

            public string Status
            {
                get { return _status; }
            }

            public string Text
            {
                get { return _text; }
            }

            public long Time
            {
                get { return _time; }
            }
        }
    }

    public class TimedStep : IComparable<TimedStep>
    {
        public string Text { get; set; }

        public long Start { get; set; }

        public long Finished { get; set; }

        public long Duration()
        {
            return Finished - Start;
        }

        public int CompareTo(TimedStep other)
        {
            // reverse the ordering if finished is the same
            if (other.Finished == Finished) return other.Start.CompareTo(Start);

            return Finished.CompareTo(other.Finished);
        }
    }
}