using System;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CanBeMultiplesAttribute : Attribute
    {
    }
}