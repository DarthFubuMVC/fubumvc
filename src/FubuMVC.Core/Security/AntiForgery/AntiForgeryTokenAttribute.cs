using System;

namespace FubuMVC.Core.Security.AntiForgery
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AntiForgeryTokenAttribute : Attribute
    {
        public string Salt { get; set; }
    }
}