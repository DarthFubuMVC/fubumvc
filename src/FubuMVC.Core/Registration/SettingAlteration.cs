using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration
{
    [ConfigurationType(ConfigurationType.Settings)]
    public class SettingAlteration<T> : IConfigurationAction, DescribesItself where T : class
    {
        private readonly Action<T> _alteration;

        public SettingAlteration(Action<T> alteration)
        {
            _alteration = alteration;
        }

        public void Describe(Description description)
        {
            description.Title = "Alter " + typeof (T).Name;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Settings.Alter(_alteration);
        }
    }
}