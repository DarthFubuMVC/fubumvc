using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly specify the url pattern for the chain containing this method
    /// as its ActionCall.  Supports inputs like "folder/find/{Id}"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UrlPatternAttribute : Attribute
    {
        private readonly string _pattern;

        public UrlPatternAttribute(string pattern)
        {
            _pattern = pattern.Trim();
        }

        public string Pattern
        {
            get { return _pattern; }
        }
    }
}