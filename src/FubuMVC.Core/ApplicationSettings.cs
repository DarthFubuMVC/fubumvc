using System;
using System.IO;
using FubuCore;
using FubuCore.Configuration;

namespace FubuMVC.Core
{
    /// <summary>
    /// Describes a FubuMVC application for simple hosts and test automation infrastructure
    /// </summary>
    [Serializable]
    public class ApplicationSettings
    {
        public ApplicationSettings()
        {
            Port = 5500;
            ParentFolder = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// The root directory of the application, equivalent to FubuMvcPackageFacility.PhysicalPath
        /// </summary>
        public string PhysicalPath { get; set; }

        /// <summary>
        /// The root url of the application.  This is primarily used for test automation scenarios
        /// where the FubuMVC application is hosted separately from the automated test code
        /// </summary>
        public string RootUrl { get; set; }


        public string Name { get; set; }
        
        /// <summary>
        /// Default is 5500
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Needs to be an assembly qualified name of a class implementing IApplicationSource
        /// </summary>
        public string ApplicationSourceName { get; set; }

        /// <summary>
        /// Reads an Xml serialized ApplicationSettings object from a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ApplicationSettings Read(string file)
        {
            var settingsData = SettingsData.ReadFromFile(SettingCategory.core, file);
            var settings = SettingsProvider.For(settingsData).SettingsFor<ApplicationSettings>();

            settings.ParentFolder = file.ToFullPath().ParentDirectory();

            return settings;
        }

        /// <summary>
        /// The parent folder containing an ApplicationSettings object loaded from the file system
        /// </summary>
        public string ParentFolder { get; set; }

        /// <summary>
        /// Reads the ApplicationSettings for a named IApplicationSource
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ApplicationSettings ReadByName(string name)
        {
            var file = GetFileNameFor(name);
            var parentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            file = parentDirectory.AppendPath(file);

            if (File.Exists(file))
            {
                return Read(file);
            }

            parentDirectory = parentDirectory.ParentDirectory();
            file = parentDirectory.AppendPath(file);

            if (File.Exists(file))
            {
                return Read(file);
            }

            parentDirectory = parentDirectory.ParentDirectory();
            file = parentDirectory.AppendPath(file);

            if (File.Exists(file))
            {
                return Read(file);
            }

            return null;
        }

        /// <summary>
        /// Reads the ApplicationSettings for a named IApplicationSource
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ApplicationSettings ReadFor<T>() where T : IApplicationSource
        {
            return ReadByName(typeof (T).Name.Replace("Application", ""));
        }

        /// <summary>
        /// Determines the application folder for the application described by this settings class
        /// </summary>
        /// <returns></returns>
        public string GetApplicationFolder()
        {
            if (PhysicalPath.IsEmpty())
            {
                return ParentFolder.IsEmpty()
                           ? AppDomain.CurrentDomain.BaseDirectory
                           : ParentFolder;
            }

            if (Path.IsPathRooted(PhysicalPath)) return PhysicalPath;

            return ParentFolder.ToFullPath().AppendPath(PhysicalPath).ToFullPath();
        }

        /// <summary>
        /// Writes this instance of ApplicationSettings back to the original filename
        /// </summary>
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

        
        /// <summary>
        /// Returns the absolute path for this ApplicationSettings by its name
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            var file = GetFileNameFor(Name);
            return ParentFolder.AppendPath(file);
        }

        /// <summary>
        /// Returns only the file name for this ApplicationSettings
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public static string GetFileNameFor(string applicationName)
        {
            return applicationName + ".application.config";
        }

        /// <summary>
        /// Builds the default ApplicationSettings object for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ApplicationSettings For<T>() where T : IApplicationSource
        {
            return new ApplicationSettings{
                ApplicationSourceName = typeof(T).AssemblyQualifiedName,
                Name = typeof(T).Name.Replace("Application", ""),
                RootUrl = "http://localhost/" + typeof(T).Name.Replace("Application", "").ToLower()

            };
        }

        /// <summary>
        /// FileSet that searches for all serialized ApplicationSetting objects
        /// </summary>
        /// <returns></returns>
        public static FileSet FileSearch()
        {
            return new FileSet{
                DeepSearch = true,
                Include = "*.application.config"
            };
        }

        /// <summary>
        /// Creates a FileSet to search for the ApplicationSettings for a specific application
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
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