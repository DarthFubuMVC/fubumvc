using System;

namespace FubuMVC.Core.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class NotAuthenticatedAttribute : Attribute
    {
        
    }
}