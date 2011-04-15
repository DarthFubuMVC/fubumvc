using System.Xml.Serialization;

namespace Bottles.Services
{
    /// <summary>
    /// ~/
    ///     control/
    ///         .manifest
    ///     data/
    ///         [stuff]
    /// </summary>
    [XmlType("manifest")]
    public class HostManifest
    {
        public static string FILE = ".manifest";

        public static string CONTROL = "control";
        public static string DATA = "data";

        public string Name { get; set; }
        public string Bootstrapper { get; set; }
    }
}