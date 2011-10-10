using System;
using FubuCore;
using FubuCore.Configuration;

namespace Serenity
{
    public class ApplicationSettings
    {
        public string PhysicalPath { get; set; }
        public string RootUrl { get; set; }
        public string Name { get; set; }

        public static ApplicationSettings Read(string file)
        {
            var settings = SettingsData.ReadFromFile(SettingCategory.core, file);
            return SettingsProvider.For(settings).SettingsFor<ApplicationSettings>();
        }

        public static ApplicationSettings ReadByName(string name)
        {
            var file = name + ".application";
            file = AppDomain.CurrentDomain.BaseDirectory.AppendPath(file);

            return Read(file);
        }

        public void Write()
        {
            var file = Name + ".application";
            file = AppDomain.CurrentDomain.BaseDirectory.AppendPath(file);

            new FileSystem().AlterFlatFile(file, list =>
            {
                list.Clear();

                list.Add("ApplicationSettings.PhysicalPath=" + PhysicalPath);
                list.Add("ApplicationSettings.PhysicalPath=" + PhysicalPath);
                list.Add("ApplicationSettings.PhysicalPath=" + PhysicalPath);
            });
        }
    }
}