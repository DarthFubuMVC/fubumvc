using System.ComponentModel;
using FubuCore.CommandLine;

namespace FubuMVC.Core.Packaging
{
    public class CreatePackageInput
    {
        [Description("The root physical folder (or valid alias) of the package")]
        public string PackageFolder { get; set; }
        
        [Description("The location where the zip file for the package will be written")]
        public string ZipFile { get; set; }

        [Description("Includes any matching .pdb files for the package assemblies")]
        public bool PdbFlag { get; set; }

        [Description("Forces the command to delete any existing zip file first")]
        [FlagAlias("f")]
        public bool ForceFlag { get; set; }
    }
}