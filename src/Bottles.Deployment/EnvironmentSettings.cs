using System;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;
using FubuCore.Configuration;
using FubuCore.Util;
using System.Collections.Generic;

namespace Bottles.Deployment
{
    [Serializable]
    public class EnvironmentSettingsException : Exception
    {
        private static readonly string _validUsage =
            "Environment settings must be in the form '[Prop]=[Value]' or '[Host].[Directive].[Property]=[Value], but was\n{0}";


        public EnvironmentSettingsException(string actual) : base(_validUsage.ToFormat(actual))
        {
        }

        protected EnvironmentSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class EnvironmentSettings
    {
        private readonly Cache<string, string> _overrides = new Cache<string, string>();
        private readonly Cache<string, InMemorySettingsData> _settings = new Cache<string, InMemorySettingsData>(name => new InMemorySettingsData(SettingCategory.environment));


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

        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        public Cache<string, string> Overrides
        {
            get { return _overrides; }
        }

        public InMemorySettingsData DataForHost(string hostName)
        {
            return _settings[hostName];
        }

    }
}