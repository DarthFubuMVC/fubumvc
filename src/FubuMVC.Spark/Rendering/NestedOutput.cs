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
            _writerSource = writer;
        }

        public TextWriter Writer
        {
            get
            {
                var writer = _writerSource();
                _isActive = true;
                return writer;
            }
        }

        public bool IsActive()
        {
            return _isActive;
        }
    }
}