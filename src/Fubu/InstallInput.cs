using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuMVC.Core.Packaging;

namespace Fubu
{
    public class InstallInput
    {
        public InstallInput()
        {
            ModeFlag = InstallMode.install;
            LogFileFlag = "installation.htm";
        }

        [Description("Root folder (or alias) of the fubu application")]
        public string AppFolder { get; set; }

        [Description("Determines what actions are executed for each installer.  'install' is the default")]
        public InstallMode ModeFlag { get; set; }

        [Description("Overrides the location of the log file produced, otherwise 'installation.htm' is the default")]
        public string LogFileFlag { get; set; }

        [Description("When specified, opens the resulting log file in the default web browser")]
        public bool OpenFlag { get; set; }

        public string ManifestFileName
        {
            get { return Path.GetFullPath(FileSystem.Combine(AppFolder, ApplicationManifest.FILE)); }
        }

        public string Title()
        {
            var format = "";

            switch (ModeFlag)
            {
                case InstallMode.install:
                    format = "Installing the application at {0}";
                    break;

                case InstallMode.check:
                    format = "Running environment checks for {0}";
                    break;

                case InstallMode.all:
                    format = "Installing and running environment checks for {0}";
                    break;
            }

            return format.ToFormat(ManifestFileName);
        }
    }
}