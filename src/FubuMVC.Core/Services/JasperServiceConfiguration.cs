using System.Xml.Serialization;

namespace FubuMVC.Core.Services
{
    [XmlType("service")]
    public class JasperServiceConfiguration
    {
        public const string FILE = "jasper-service.config";

        public JasperServiceConfiguration()
        {
            Name = "JasperService";
            DisplayName = "JasperService";
            Description = "JasperService";

			// Hides command line output so only turn this on when running as a service
	        UseEventLog = false;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
		public bool UseEventLog { get; set; }
        public string BootstrapperType { get; set; }
    }
}