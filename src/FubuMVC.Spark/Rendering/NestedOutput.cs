using System;
using System.IO;

namespace FubuMVC.Spark.Rendering
{
    public class NestedOutput
    {
        private Func<TextWriter> _writerSource;
        private bool _isActive;

        public void SetWriter(Func<TextWriter> writer)
        {
            _isActive = true;
            _writerSource = writer;
        }

        public TextWriter Writer
        {
            get { return _writerSource(); }
        }

        public bool IsActive()
        {
            return _isActive;
        }
    }
}