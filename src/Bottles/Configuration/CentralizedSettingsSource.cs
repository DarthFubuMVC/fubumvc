using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FubuCore;
using FubuCore.Configuration;

namespace Bottles.Configuration
{
    public class CentralizedSettingsSource : ISettingsSource
    {
        private readonly string _folder;

        public CentralizedSettingsSource(string folder)
        {
            _folder = folder;
        }

        public IEnumerable<SettingsData> FindSettingData()
        {
            var environmentFile = FileSystem.Combine(_folder, EnvironmentSettings.EnvironmentSettingsFileName);
            var settings = EnvironmentSettings.ReadFrom(environmentFile);

            var substitutions = settings.Overrides.ToDictionary();

            yield return settings.EnvironmentSettingsData();

            foreach (var file in Directory.GetFiles(_folder, "*.config"))
            {
                yield return XmlSettingsParser.Parse(file, substitutions);
            }

        }
    }
}