using System;
using System.IO;

namespace FubuMVC.Spark.Rendering
{
    public class NestedOutput
    {
        private Func<TextWriter> _writer;

        public void SetWriter(Func<TextWriter> writer)
        {
            _writer = writer;
        }
        public TextWriter Writer { get { return _writer(); } }

        public bool IsActive()
        {
            return _writer != null;
        }
    }
}