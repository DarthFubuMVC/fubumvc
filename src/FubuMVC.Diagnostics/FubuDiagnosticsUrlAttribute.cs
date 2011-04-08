using System;

namespace FubuMVC.Diagnostics
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FubuDiagnosticsUrlAttribute : Attribute
    {
        private readonly string _url;

        public FubuDiagnosticsUrlAttribute(string url)
        {
            _url = url;
        }

        public string Url
        {
            get { return _url; }
        }
    }
}