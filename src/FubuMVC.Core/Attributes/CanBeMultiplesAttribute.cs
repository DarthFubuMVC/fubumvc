using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Only used on IConfigurationAction classes.  Forces FubuMVC to accept multiple
    /// instances of an IConfigurationAction type in Policies.Add()
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CanBeMultiplesAttribute : Attribute
    {
    }
}