using System;

namespace FubuMVC.Core
{
    [Flags]
    public enum FormatterOptions
    {
        Html = 1,
        Json = 2,
        Xml = 4,
        All = 8
    }
}