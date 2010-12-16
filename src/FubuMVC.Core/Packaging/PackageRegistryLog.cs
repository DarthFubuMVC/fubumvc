using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    [Serializable]
    public class PackageRegistryLog : IPackageLog
    {
        private readonly StringWriter _text = new StringWriter();
        private readonly IList<object> _children = new List<object>();

        public PackageRegistryLog()
        {
            Success = true;
            Id = Guid.NewGuid();
        }

        public long TimeInMilliseconds { get; set; }
        public string Provenance { get; set; }
        public string Description { get; set; }

        public void Trace(string text)
        {
            _text.WriteLine(text);
        }

        public void Trace(string format, params object[] parameters)
        {
            Trace(format.ToFormat(parameters));
        }

        public bool Success { get; private set; }

        public void MarkFailure(Exception exception)
        {
            MarkFailure(exception.ToString());
        }

        public void MarkFailure(string text)
        {
            Trace(text);
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

        public Guid Id
        {
            get; private set;
        }
    }
}