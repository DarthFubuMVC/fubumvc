using System;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ConfigurationTypeAttribute : Attribute
    {
        private readonly string _type;

        public ConfigurationTypeAttribute(string type)
        {
            _type = type;
        }

        public string Type
        {
            get { return _type; }
        }
    }
}