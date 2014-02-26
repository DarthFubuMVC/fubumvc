using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly marks this endpoint as "symmetric Json," meaning that it
    /// will only accept Json and output to Json
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SymmetricJsonAttribute : Attribute
    {
    }
}