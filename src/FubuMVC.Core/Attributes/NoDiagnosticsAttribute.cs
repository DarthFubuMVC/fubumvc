using System;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDiagnosticsAttribute : Attribute
    {
        
    }
}