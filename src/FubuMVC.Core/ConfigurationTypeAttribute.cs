using System;

namespace FubuMVC.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ConfigurationTypeAttribute : Attribute
    {
        private readonly ConfigurationType _configurationType;

        public ConfigurationTypeAttribute(ConfigurationType configurationType)
        {
            _configurationType = configurationType;
        }

        public ConfigurationType ConfigurationType
        {
            get { return _configurationType; }
        }
    }
}