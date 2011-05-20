using System;
using System.IO;

namespace FubuMVC.Spark.Rendering
{
    public class NestedOutput
    {
        private Func<TextWriter> _writer;
        private bool _isActive;

        public void SetWriter(Func<TextWriter> writer)
        {
            _writer = writer;
            _isActive = true;
        }

        public TextWriter Writer
        {
            get { return _writer(); }
        }
		
        public bool IsActive()
        {
            return _isActive;
        }
    }
}