using System;
using System.Linq;
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
        private readonly Cache<string, SettingsData> _settings = new Cache<string, SettingsData>(name => new SettingsData(SettingCategory.environment));


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
            else if (directiveParts.Length >= 3)
            {
                var hostName = directiveParts.First();
                var propertyName = directiveParts.Skip(1).Join(".");

                _settings[hostName][propertyName] = value;
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
            return _settings[hostName];
        }

        public SettingsData EnvironmentSettingsData()
        {
            throw new NotImplementedException();
        }

    }
}