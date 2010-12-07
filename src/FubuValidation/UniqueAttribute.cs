using System;

namespace FubuValidation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : Attribute
    {
        public string Key { get; set; }
    }
}