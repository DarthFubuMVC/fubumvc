using System.ComponentModel;
using FubuCore.CommandLine;

namespace Fubu
{
    public class InstallPackageInput
    {
        [Description("The package zip file location to be installed.  If un-installing, just use the zip file name")]
        [RequiredUsage("install", "uninstall")]
        public string PackageFile { get; set; }

        [Description("The physical folder (or valid alias) of the main application")]
        [RequiredUsage("install", "uninstall")]
        public string AppFolder { get; set; }

        [FlagAlias("uninstall", 'u')]
        [RequiredUsage("uninstall")]
        [ValidUsage("uninstall")]
        [Description("Uninstalls the named package from an application folder")]
        public bool UninstallFlag { get; set; }
    }
}