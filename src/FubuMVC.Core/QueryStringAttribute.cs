using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Marks a property as bound to the querystring so that a non-default
    /// value of the marked property will be added to the generated url
    /// in IUrlRegistry
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryStringAttribute : Attribute
    {
    }
}