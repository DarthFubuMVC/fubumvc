using System;

namespace FubuMVC.Diagnostics
{
    public class BindFromAttribute : Attribute
    {
        public BindFromAttribute(string source)
        {
            Source = source;
        }

        public string Source { get; private set; }
    }
}