using System;

namespace FubuMVC.Diagnostics
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDiagnosticsAttribute : Attribute
    {
        
    }
}