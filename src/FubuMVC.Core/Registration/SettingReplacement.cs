using FubuCore.Descriptions;

namespace FubuMVC.Core.Registration
{
    [ConfigurationType(ConfigurationType.Settings)]
    public class SettingReplacement<T> : IConfigurationAction, DescribesItself
    {
        private readonly T _settings;

        public SettingReplacement(T settings)
        {
            _settings = settings;
        }

        public void Configure(BehaviorGraph graph)
        {
            graph.Settings.Replace(_settings);
        }

        public void Describe(Description description)
        {
            description.Title = "Replace the settings for " + typeof (T).Name;
        }
    }
}