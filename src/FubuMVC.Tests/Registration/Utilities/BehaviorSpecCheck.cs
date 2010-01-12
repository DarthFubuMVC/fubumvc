using System.IO;
using System.Text;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Utilities
{
    public class BehaviorSpecCheck
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly StringWriter _writer;
        private bool _hasErrors;
        private int _level;

        public BehaviorSpecCheck()
        {
            _writer = new StringWriter(_builder);
        }

        public void AssertBehaviors()
        {
            if (_hasErrors)
            {
                Assert.Fail(_builder.ToString());
            }
        }

        public void Write(string description)
        {
            newLine();

            _writer.Write(description);
        }

        private void newLine()
        {
            _writer.WriteLine();
            for (int i = 0; i < _level; i++)
            {
                _writer.Write("  ");
            }
        }

        public void RegisterError(string error)
        {
            _hasErrors = true;
            newLine();
            _writer.Write("  Error:  " + error);
        }

        public void Increment()
        {
            _level++;
        }

        public void Decrement()
        {
            _level++;
        }
    }
}