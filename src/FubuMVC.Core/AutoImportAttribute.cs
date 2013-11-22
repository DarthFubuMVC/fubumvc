using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Apply to an IConfigurationAction or IFubuRegistryExtension class
    /// in the application assembly to have it automatically applied
    /// to the application.  Mostly to support template generated 
    /// code
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutoImportAttribute : Attribute
    {
        
    }
}