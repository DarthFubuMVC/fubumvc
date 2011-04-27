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
            //var settings = new EnvironmentSettings(_folder);
            //var substitutions = settings.Overrides.ToDictionary();

            //yield return settings.EnvironmentSettingsData();

            //foreach (var file in Directory.GetFiles(_folder, "*.config"))
            //{
            //    var document = new XmlDocument();
            //    document.Load(file);


            //}
            throw new NotImplementedException();
        }
    }
}