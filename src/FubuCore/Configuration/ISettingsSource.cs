using System.Collections.Generic;

namespace FubuCore.Configuration
{
    public interface ISettingsSource
    {
        IEnumerable<ISettingsData> FindSettingData();
    }

    public class SettingsSource : ISettingsSource
    {
        private readonly IList<ISettingsData> _settings = new List<ISettingsData>();

        public SettingsSource(IEnumerable<ISettingsData> settings)
        {
            _settings.AddRange(settings);
        }

        public IEnumerable<ISettingsData> FindSettingData()
        {
            return _settings;
        }

        public void Add(ISettingsData data)
        {
            _settings.Add(data);
        }

        public static SettingsSource For(params ISettingsData[] data)
        {
            return new SettingsSource(data);
        }
    }
}