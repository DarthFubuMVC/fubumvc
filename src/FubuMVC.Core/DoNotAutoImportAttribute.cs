using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Use this attribute on an IFubuRegistryExtension class to 
    /// prevent FubuMVC from automatically applying the extension
    /// if it is contained in a Bottle assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DoNotAutoImportAttribute : Attribute
    {
         
    }
}