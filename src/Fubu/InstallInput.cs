using System.IO;
using FubuCore;
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

        public string AppFolder { get; set; }
        public InstallMode ModeFlag { get; set; }
        public string LogFileFlag { get; set; }
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