using System.ComponentModel;
using FubuCore.CommandLine;

namespace Fubu
{
    public class ManifestInput
    {
        [Description("Physical folder (or valid alias) of the application")]
        public string AppFolder { get; set; }

        [Description("Creates a new application manifest file for the application")]
        public bool CreateFlag { get; set; } // creates, but does not override

        [Description("The main environment class name in assembly qualified form for usage with the 'install' command")]
        [FlagAlias("class")]
        public string EnvironmentClassNameFlag { get; set; }

        [Description("Write the main application assembly to the manifest file for usage with the 'install command")]
        public string AssemblyFlag { get; set; }

        [Description("Opens the manifest file in notepad")]
        public bool OpenFlag { get; set; }

        [Description("Force the command to overwrite any existing manifest file if using the -create flag")]
        [FlagAlias("f")]
        public bool ForceFlag { get; set; }

        
    }
}