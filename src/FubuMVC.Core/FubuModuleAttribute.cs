using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// FubuMVC applications will treat any assembly marked with the 
    /// [FubuModule] attribute as a Bottle
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class FubuModuleAttribute : Attribute
    {
    }
}