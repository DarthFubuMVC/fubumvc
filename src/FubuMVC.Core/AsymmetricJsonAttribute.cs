using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly marks this endpoint as asymmetric Json, meaning that it
    /// accepts either form posts or Json posts and outputs json
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AsymmetricJsonAttribute : Attribute
    {
    }
}