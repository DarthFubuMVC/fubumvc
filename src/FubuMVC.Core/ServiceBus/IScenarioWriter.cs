using System;
using System.Collections.Generic;
using System.IO;

namespace FubuMVC.Core.ServiceBus
{
    public interface IScenarioWriter
    {
        IDisposable Indent();

        void WriteLine(string format, params object[] parameters);
        void WriteTitle(string title);
        void BlankLine();

        void Exception(Exception ex);
        void Failure(string format, params object[] parameters);
        void Bullet(string format, params object[] parameters);
    }

    public class ScenarioWriter : IScenarioWriter
    {
        private int _indent = 0;
        private readonly StringWriter _writer = new StringWriter();

        public IDisposable Indent()
        {
            _indent += 4;
            return new Indention(this);
        }

        public class Indention : IDisposable
        {
            private readonly ScenarioWriter _parent;

            public Indention(ScenarioWriter parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                _parent._indent -= 4;
            }
        }

        public void WriteLine(string format, params object[] parameters)
        {
            _writer.Write(string.Empty.PadLeft(_indent));
            _writer.WriteLine(format, parameters);

            Console.Write(string.Empty.PadLeft(_indent));
            Console.WriteLine(format, parameters);
        }

        public void WriteTitle(string title)
        {
            _indent = 0;
            WriteLine(title);
        }

        public void BlankLine()
        {
            Console.WriteLine();
            _writer.WriteLine();
        }

        public void Exception(Exception ex)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            var lines = ex.ToString().Split(System.Environment.NewLine.ToCharArray());
            lines.Each(x => WriteLine(x));

            FailureCount++;

            Console.ForegroundColor = color;
        }

        public void Failure(string format, params object[] parameters)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            WriteLine("FAILURE:  " + format, parameters);

            FailureCount++;
            Console.ForegroundColor = color;
        }

        public void Bullet(string format, params object[] parameters)
        {
            WriteLine("* " + format, parameters);
        }

        public int FailureCount { get; private set; }

        public override string ToString()
        {
            return _writer.ToString();
        }
    }
}