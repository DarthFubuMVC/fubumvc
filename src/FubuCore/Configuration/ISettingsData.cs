using System.Collections.Generic;

namespace FubuCore.Configuration
{
    public interface ISettingsData
    {
        string Description { get; }
        SettingCategory Category { get; }

        bool Has(string key);
        string Get(string key);
        IEnumerable<string> AllKeys { get; }
    }
}