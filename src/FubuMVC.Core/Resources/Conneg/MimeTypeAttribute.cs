using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Resources.Conneg
{
    [MarkedForTermination("Let's get rid of this when we collapse things down")]
    [AttributeUsage(AttributeTargets.Class)]
    public class MimeTypeAttribute : Attribute
    {
        private readonly string[] _mimeTypes;

        public MimeTypeAttribute(params string[] mimeTypes)
        {
            _mimeTypes = mimeTypes;
        }

        public string[] MimeTypes
        {
            get { return _mimeTypes; }
        }

        public static IEnumerable<string> ReadFrom(Type type)
        {
            var mimeTypes = new[] { "Unknown" };
            type.ForAttribute<MimeTypeAttribute>(att => mimeTypes = att.MimeTypes);

            return mimeTypes;
        }
    }
}