using System;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration
{
    public class SettingAlteration<T> : ISettingsAlteration, DescribesItself where T : class
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

        public void Alter(SettingsCollection settings)
        {
            settings.Alter(_alteration);
        }
    }
}