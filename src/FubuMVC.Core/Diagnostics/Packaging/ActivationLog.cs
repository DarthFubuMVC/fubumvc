using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;

namespace FubuMVC.Core.Diagnostics.Packaging
{
    [Serializable]
    public class ActivationLog : IActivationLog
    {
        private readonly IPerfTimer _timer;
        private readonly StringWriter _text = new StringWriter();
        private readonly IList<object> _children = new List<object>();

        public ActivationLog(IPerfTimer timer)
        {
            _timer = timer;
            Success = true;
            Id = Guid.NewGuid();
        }

        public ActivationLog() : this(new PerfTimer())
        {
        }

        public string Provenance { get; set; }
        public string Description { get; set; }

        public void Execute(Action continuation)
        {
            try
            {
                _timer.Record(Description, continuation);
            }
            catch (Exception e)
            {
                MarkFailure(e);
            }
        }

        public void Trace(ConsoleColor color, string format, params object[] parameters)
        {
            var text = format;

            if (parameters.Length > 0)
            {
                try
                {
                    text = format.ToFormat(parameters);
                }
                catch (FormatException ex)
                {
                    //this could be moved into 'ToFormat'
                    var f = format.Replace("{", "{{").Replace("}", "}}");
                    var a = parameters.Aggregate((l, r) => l + "," + r);
                    throw new Exception("Attempted to format the string --{0}-- with --{1}--".ToFormat(f, a), ex);
                }
            }

            _text.WriteLine(text);
        }

        public void Trace(string format, params object[] parameters)
        {
            Trace(ConsoleColor.Gray, format, parameters);
        }

        public bool Success { get; private set; }

        public void MarkFailure(Exception exception)
        {
            MarkFailure(exception.ToString());
        }

        public void MarkFailure(string text, params object[] args)
        {
            var output = args.Any() ? text.ToFormat(args) : text;
            ConsoleWriter.Write(ConsoleColor.Red, output);

            _text.WriteLine(output);

            Success = false;
        }

        public string FullTraceText()
        {
            return _text.ToString();
        }

        public void AddChild(params object[] child)
        {
            _children.AddRange(child);
        }

        public IEnumerable<T> FindChildren<T>()
        {
            return _children.Where(x => x is T).Cast<T>();
        }


        public Guid Id { get; private set; }


        public void TrapErrors(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                MarkFailure(e);
            }
        }
    }
}