using System;
using System.IO;
using FubuCore;
using FubuCore.Configuration;

namespace FubuMVC.Core
{
    [Serializable]
    public class ApplicationSettings
    {
        public ApplicationSettings()
        {
            Port = 5500;
            ParentFolder = AppDomain.CurrentDomain.BaseDirectory;
        }

        public string PhysicalPath { get; set; }
        public string RootUrl { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }

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
            var file = GetFileName();

            new FileSystem().AlterFlatFile(file, list =>
            {
                list.Clear();

                if (PhysicalPath != null) list.Add("ApplicationSettings.PhysicalPath=" + PhysicalPath);
                if (ApplicationSourceName != null)
                    list.Add("ApplicationSettings.ApplicationSourceName=" + ApplicationSourceName);
                list.Add("ApplicationSettings.Port=" + Port);
                if (Name != null) list.Add("ApplicationSettings.Name=" + Name);
                if (RootUrl != null) list.Add("ApplicationSettings.RootUrl=" + RootUrl);
            });
        }

        

        public string GetFileName()
        {
            var file = GetFileNameFor(Name);
            return ParentFolder.AppendPath(file);
        }

        public static string GetFileNameFor(string applicationName)
        {
            return applicationName + ".application.config";
        }

        public static ApplicationSettings For<T>() where T : IApplicationSource
        {
            return new ApplicationSettings{
                ApplicationSourceName = typeof(T).AssemblyQualifiedName,
                Name = typeof(T).Name.Replace("Application", ""),
                RootUrl = "http://localhost/" + typeof(T).Name.Replace("Application", "").ToLower()

            };
        }

        public static FileSet FileSearch()
        {
            return new FileSet{
                DeepSearch = true,
                Include = "*.application.config"
            };
        }

        public static FileSet FileSearch(string applicationName)
        {
            return new FileSet
            {
                DeepSearch = true,
                Include = GetFileNameFor(applicationName)
            };
        }
    }
}