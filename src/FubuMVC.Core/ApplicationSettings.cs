using System;
using System.IO;
using FubuCore;
using FubuCore.Configuration;

namespace FubuMVC.Core
{
    public class ApplicationSettings
    {
        public string PhysicalPath { get; set; }
        public string RootUrl { get; set; }
        public string Name { get; set; }

        // Needs to be an assembly qualified name of a class implementing IApplicationSource
        public string ApplicationSourceName { get; set; }

        public static ApplicationSettings Read(string file)
        {
            var settingsData = SettingsData.ReadFromFile(SettingCategory.core, file);
            var settings = SettingsProvider.For(settingsData).SettingsFor<ApplicationSettings>();

            settings.ParentFolder = file.ToFullPath().ParentDirectory();

            return settings;
        }

        public string ParentFolder { get; set; }

        public static ApplicationSettings ReadByName(string name)
        {
            var file = name + ".application.config";
            file = AppDomain.CurrentDomain.BaseDirectory.AppendPath(file);

            return Read(file);
        }

        public string GetApplicationFolder()
        {
            if (PhysicalPath.IsEmpty())
            {
                return ParentFolder.IsEmpty()
                           ? AppDomain.CurrentDomain.BaseDirectory
                           : ParentFolder;
            }

            if (Path.IsPathRooted(PhysicalPath)) return PhysicalPath;

            return ParentFolder.ToFullPath().AppendPath(PhysicalPath);
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