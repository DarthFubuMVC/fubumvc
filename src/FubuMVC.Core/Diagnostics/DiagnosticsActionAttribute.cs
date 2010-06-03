using System;

namespace FubuMVC.Core.Diagnostics
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DiagnosticsActionAttribute : Attribute
    {
    }
}