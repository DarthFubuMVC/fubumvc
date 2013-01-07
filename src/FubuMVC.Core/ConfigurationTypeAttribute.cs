using System;

namespace FubuMVC.Core
{
    /// <summary>
    /// Used on IConfigurationAction types to control the relative placement
    /// of an IConfigurationAction in the BehaviorGraph construction cycle
    /// </summary>
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