using System;
using System.Linq;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Util;
using System.Collections.Generic;

namespace Bottles.Configuration
{
    public class EnvironmentSettings
    {
        public static readonly string EnvironmentSettingsFileName = "environment.settings";
        public static readonly string ROOT = "root";

        private readonly Cache<string, string> _overrides = new Cache<string, string>();
        private readonly Cache<string, SettingsData> _settingsByHost = new Cache<string, SettingsData>(name => new SettingsData(SettingCategory.environment){Provenance = EnvironmentSettingsFileName});
        private readonly SettingsData _environmentSettings = new SettingsData(SettingCategory.environment){
            Provenance = "Environment settings"
        };

        public void ReadText(string text)
        {
            if (!text.Contains("="))
            {
                throw new EnvironmentSettingsException(text);
            }

            var parts = text.Split('=').Select(x => x.Trim()).ToArray();
            if (parts.Count() != 2)
            {
                throw new EnvironmentSettingsException(text);
            }

            var value = parts.Last();
            
            var directiveParts = parts.First().Split('.');
            if (directiveParts.Length == 1)
            {
                _overrides[parts.First()] = value;
            }
            else if (directiveParts.Length == 2)
            {
                _environmentSettings[parts.First()] = value;
            }
            else if (directiveParts.Length >= 3)
            {
                var hostName = directiveParts.First();
                var propertyName = directiveParts.Skip(1).Join(".");

                _settingsByHost[hostName][propertyName] = value;
            }
            else
            {
                throw new EnvironmentSettingsException(text);
            }


        }

        public Cache<string, string> Overrides
        {
            get { return _overrides; }
        }

        public SettingsData DataForHost(string hostName)
        {
            return _settingsByHost[hostName];
        }

        public SettingsData EnvironmentSettingsData()
        {
            return _environmentSettings;
        }

        public static EnvironmentSettings ReadFrom(string environmentFile)
        {
            var environment = new EnvironmentSettings();
            new FileSystem().ReadTextFile(environmentFile, environment.ReadText);

            return environment;
        }

        /// <summary>
        /// Sets {root} to targetDirectory for use in other settings files
        /// </summary>
        public void SetRootSetting(string targetDirectory)
        {
            _overrides[ROOT] = targetDirectory;
        }
    }
}