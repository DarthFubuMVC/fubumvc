using FubuCore.Configuration;

namespace Serenity
{
    public class ApplicationSettings
    {
        public string PhysicalPath { get; set; }
        public string RootUrl { get; set; }

        public static ApplicationSettings Read(string file)
        {
            var settings = SettingsData.ReadFromFile(SettingCategory.core, file);
            return SettingsProvider.For(settings).SettingsFor<ApplicationSettings>();
        }

        public static ApplicationSettings ReadByName(string name)
        {
            return Read(name + ".application");
        }
    }
}