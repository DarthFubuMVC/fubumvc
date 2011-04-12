using System.Collections.Generic;

namespace FubuCore.Configuration
{
    public interface ISettingsSource
    {
        IEnumerable<ISettingsData> FindSettingData();
    }
}