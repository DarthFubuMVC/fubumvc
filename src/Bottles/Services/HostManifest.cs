using System.Xml.Serialization;

namespace Bottles.Services
{
    [XmlType("manifest")]
    public class HostManifest
    {
        public static string FILE = ".manifest";


        //REVIEW: these are folders - where should this best go?
        public static string CONTROL = "control";
        public static string DATA = "data";

        public string Name { get; set; }
        public string Bootstrapper { get; set; }
        public string EntryBottle { get; set; }
    }
}