using System.ComponentModel;

namespace Fubu.Packages
{
    public class AssemblyPackageInput
    {
        [Description("The root folder for the project if different from the project file's folder")]
        public string RootFolder { get; set; }
        
        [Description("Name of the csproj file.  If set, this command attempts to add the zip files as embedded resources")]
        public string ProjFileFlag { get; set; }


    }
}