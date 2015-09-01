using System;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu.Running
{
    public enum BrowserType
    {
        Chrome,
        Firefox
    }

    public class ApplicationRequest
    {
        private string _directoryFlag;

        public ApplicationRequest()
        {
            PortFlag = 5500;
            DirectoryFlag = Environment.CurrentDirectory;
            BuildFlag = "Debug";
            ModeFlag = "Development";
        }

        [Description(
            "If you are running a class library, sets the preference for the profile to load.  As in bin/[BuildFlag].  Default is debug"
            )]
        public string BuildFlag { get; set; }

        [Description("IP Port.  Default is 5500")]
        public int PortFlag { get; set; }

        [Description("Specific name of a FubuRegistry class that builds this application")]
        public string RegistryFlag { get; set; } // this is optional

        [Description("If set, overrides the name of the configuration file for the FubuMVC application AppDomain to use"
            )]
        public string ConfigFlag { get; set; }

        [Description("Overrides the directory that is the physical path of the running fubumvc application")]
        public string DirectoryFlag
        {
            get { return _directoryFlag; }
            set
            {
                _directoryFlag = value.ToFullPath();
            }
        }

        // This is mandatory

        [Description("Start the default browser to the home page of this application")]
        public bool OpenFlag { get; set; }

        [Description("Open a 'watched' browser with WebDriver to refresh the page on content or application recycles")]
        public bool WatchedFlag { get; set; }

        [Description("Unless this flag is set, the fubumvc application will run in Development mode")]
        [FlagAlias("mode")]
        public string ModeFlag { get; set; }


        public string DetermineBinPath()
        {
            var buildPath = DirectoryFlag.AppendPath("bin", BuildFlag);
            if (Directory.Exists(buildPath))
            {
                return Path.Combine("bin", BuildFlag);
            }

            var binPath = DirectoryFlag.AppendPath("bin");
            if (Directory.Exists(binPath))
            {
                return "bin";
            }

            return null;
        }

        [IgnoreOnCommandLine]
        public string AutoRefreshWebSocketsAddress { get; set; }

        [Description("Set an optional relative url to open first")]
        public string UrlFlag { get; set; }
    }
}