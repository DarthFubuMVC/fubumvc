using System.Xml.Serialization;
using FubuCore;

namespace Bottles.Services
{
    /// <summary>
    /// Packages look like
    /// ~/
    ///     data/
    ///     bin/
    ///         [assemblies]
    ///     config/
    ///         [config files]
    /// </summary>
    [XmlType("package")]
    public class ServicePackageManifest
    {
        public static readonly string FILE = ".package-manifest";

        public static string CONTROL = "control";
        public static string DATA = "data";
        public static string CONFIG = "config";


        public FileSet Bin { get; set; }
        public FileSet Data { get; set; }
        public FileSet Config { get; set; }


        public string Name { get; set; }
        public string Bootstrapper { get; set; }
    }
}