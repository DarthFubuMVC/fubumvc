using System;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Marks a setting class as only being valid on the top level registry/settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ApplicationLevelAttribute : Attribute
    {
        
    }
}