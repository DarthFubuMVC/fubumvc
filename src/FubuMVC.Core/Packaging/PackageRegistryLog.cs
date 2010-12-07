﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Packaging
{
    public class PackageRegistryLog : IPackageLog
    {
        private readonly StringWriter _text = new StringWriter();
        private readonly IList<object> _children = new List<object>();

        public PackageRegistryLog()
        {
            Success = true;
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
            ExceptionText += "\n\n" + exception.ToString();
            Success = false;
        }

        public string ExceptionText { get; private set; }

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
    }
}